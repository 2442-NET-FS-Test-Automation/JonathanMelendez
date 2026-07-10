# DarkKitchen Order Fulfillment Service

**DarkKitchen** is a restaurant‑style order fulfillment service built with **ASP.NET Core Minimal API**, **Entity Framework Core**, and **SQL Server**. It manages a catalog of dishes, each composed of ingredients with limited stock. Orders come in as bursts (multiple customer orders at once) or single and the service fulfills them **concurrently** while guaranteeing **never to oversell** – no order is ever fulfilled if it would take any ingredient below zero.

The system exposes a **Minimal‑API** surface for operators to:
- Seed and reset inventory
- Submit single orders or bursts of up to 100 orders
- Inspect inventory, orders, and customers
- Run reports (top dishes, top customers, fulfillment rate)
- Benchmark sequential vs parallel processing

**Orders are multi‑line** – an order can contain several dishes, each with a quantity. Fulfillment is **all‑or‑nothing** in one database transaction.

The main engineering challenge is ensuring that many orders can be processed concurrently **without ever overselling inventory**, while still maintaining high throughput.

---

## Techniques → Code Mapping (Coverage Contract)

| Technique                               | Where it’s demonstrated                                                                                                   |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| **EF Core code‑first model**            | `DarkKitchenDbContext.cs` – all entities, `DbSet<T>`                                                                      |
| **Data Annotation**                     | `[Required]`, `[MaxLength]`, `[Precision]` on `Customer`, `Dish`, etc.                                                    |
| **Fluent API mapping**                  | `OnModelCreating` – `IsRowVersion()`, `HasDefaultValueSql()`                                                              |
| **DbContext registration in DI**        | `Program.cs` – `AddDbContextFactory`                                                                                      |
| **Migrations + seed**                   | `DarkKitchenDbContext` – `HasData` for all base records                                                                   |
| **RowVersion concurrency token**        | `Ingredient.RowVersion` with `IsRowVersion()`                                                                             |
| **Minimal API endpoints**               | All `app.MapGet/MapPost/MapDelete` in `Program.cs`                                                                        |
| **Model binding**                       | Route parameters, query strings, and JSON bodies (`OrderPayload`, etc.)                                                   |
| **Status codes**                        | 200, 201, 202, 400, 404 – see the endpoint list                                                                           |
| **SQL 3NF + FKs**                       | All entities have foreign keys (e.g., `Order.CustomerId`, `OrderLine.DishId`)                                             |
| **Non‑key index**                       | `Ingredient.Name` is indexed  – added in migration)                                                                       |
| **One transaction per order**           | `FulfillOneAsync` uses a single `SaveChangesAsync` for each order                                                         |
| **ACID / isolation reasoning**          | See section below                                                                                                         |
| **Concurrent burst**                    | `FulfillBurstAsync` uses `Task.WhenAll` with per‑order `DbContext`                                                        |
| **Oversell prevention**                 | `SaveWithRetryAsync` uses optimistic concurrency and retry – reloads all ingredients, re‑checks sufficiency, then deducts |
| **Retry logic**                         | `SaveWithRetryAsync` – until the stock is not enough.                                                                     |
| **CancellationToken graceful shutdown** | `lifetime.ApplicationStopping` passed to background task; shutdown waits up to 30s for burst to finish                    |
| **Expedited priority queue**            | `OrderByPriority` uses `PriorityQueue<int, int>` – urgent orders (priority 0) before normal (priority 1)                  |
| **ConcurrentDictionary**                | `DarkKitchenRepoSqlServer` – used to cache ingredient lookups thread‑safely                                               |
| **Sorted report + binary search**       | `/reports/dishes/ranking/{dishName}` – ranks dishes by units sold, then binary‑searches by name                           |
| **Hash‑based lookups**                  | All repository methods use `Contains` on lists (translated to SQL `IN`) and `Dictionary` for ingredient requirements      |
| **Repository behind interface**         | `IDarkKitchenRepo` and `IReportsRepo` – implemented by `DarkKitchenRepoSqlServer` / `ReportsRepoSqlServer`                |
| **Factory pattern**                     | `OrderFactory` and `CustomerFactory` build entities with validation/defaults                                              |
| **Custom exception**                    | `DishNotFoundException`, `CustomerNotFoundException` – carry data (the missing ID)                                        |
| **Serilog structured logging**          | `Log.Information("Fulfilled {OrderId}", orderId)` – no string concatenation                                               |
| **Benchmark**                           | `/benchmark` – runs same burst sequentially and concurrently, resets stock between runs, prints timings and speedup       |

---

## Big‑O Analysis

| Operation                            | Data structure / Algorithm                                                                    | Big‑O                                            |
| ------------------------------------ | --------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| **Priority queue (expedited first)** | `PriorityQueue<int, int>` – enqueue O(log n), dequeue O(log n)                                | O(log n) per order                               |
| **Hash‑based lookups**               | `Dictionary<int, decimal>` for ingredient requirements – O(1) average                         | O(1)                                             |
| **Report sort**                      | EF Core translates to SQL `ORDER BY` – database index helps; result set is sorted by units    | O(m log m) where m is number of dishes/customers |
| **Binary search on ranked dishes**   | `List.BinarySearch` with custom comparer                                                      | O(log n)                                         |
| **Burst fulfillment**                | `Task.WhenAll` – each order O(k) where k = number of ingredients in the order; total O(n * k) | O(n * k)                                         |

---

## ACID / Isolation Reasoning

Each order is fulfilled inside a **single database transaction** (implicitly through `SaveChangesAsync`). This guarantees:
- **Atomicity** – either all ingredient stock is deducted and the order marked fulfilled, or nothing changes.
- **Consistency** – stock never goes negative because we check before deducting.
- **Isolation** – we rely on **Read Committed** (default SQL Server isolation) plus optimistic concurrency via `RowVersion`. If two transactions race, the one that loses gets a `DbUpdateConcurrencyException` and retries, re‑evaluating stock with fresh data.
- **Durability** – once `SaveChangesAsync` commits, stock is persisted.

---

## Benchmark Results

From running `/benchmark` with a 100‑order burst (mixed dishes, 3 customers, 10 dish types) on local SQL Server:

- **Sequential**: 2916 ms

Uncapped Concurrency:
- **Concurrent**: 26216 ms
- **Speedup**: 0.11x 

Capped concurrency (2):
- **Concurrent**: 2095 ms
- **Speedup**: 1.39x 

Capped concurrency (20):
- **Concurrent**: 8653 ms
- **Speedup**: 0.34x 

*Note: under heavy contention (many orders competing for the same ingredients), the speedup is limited because retries become more frequent – this is expected and demonstrates the trade‑off between parallelism and resource contention.*

---

## Status Codes Produced by Endpoints

| Code            | Meaning                                  | Example                                         |
| --------------- | ---------------------------------------- | ----------------------------------------------- |
| 200 OK          | Successful read or operation             | `GET /inventory`, `POST /inventory/reset`       |
| 201 Created     | Resource created                         | `POST /customers`, `POST /orders/single`        |
| 202 Accepted    | Burst accepted for background processing | `POST /orders/burst`                            |
| 400 Bad Request | Invalid input (unknown customer/dish)    | `POST /orders/single` with invalid `CustomerId` |
| 404 Not Found   | Resource not found                       | `GET /orders/{id}` with non‑existent ID         |
| 409 Conflict    | Not used (no conflicting state)          | –                                               |
| 204 No Content  | Empty result set                         | `GET /inventory/below-stock` when none found    |

---
# Main Features

## Concurrent Order Fulfillment

Orders are processed concurrently using asynchronous Tasks.

Each fulfillment task:

* Creates its own DbContext through IDbContextFactory
* Opens a database transaction
* Verifies ingredient availability
* Deducts inventory
* Updates order status
* Creates fulfillment events
* Commits the transaction

This guarantees that every order is processed atomically.

---

## Optimistic Concurrency

The application prevents overselling using Entity Framework Core's optimistic concurrency support.

Each Ingredient contains a RowVersion concurrency token.

If two fulfillment tasks attempt to update the same inventory row:

1. One transaction succeeds.
2. The second receives a `DbUpdateConcurrencyException`.
3. The row is reloaded.
4. Inventory is checked again.

This guarantees that inventory never becomes negative.

---

## Priority Processing

Orders are classified as:

* Urgent
* Normal

Urgent orders are processed before normal orders using a priority-based queue.

This satisfies the expedited-first requirement of the project.

---

## Reporting

The API includes several analytical endpoints:

* Top selling dishes
* Top customers
* Fulfillment rate
* Dish ranking lookup

Reports are generated using LINQ queries against SQL Server.

---

## Graceful Shutdown

The application supports graceful shutdown.

CancellationTokens are propagated through asynchronous operations.

The fulfillment background task is tracked using `IHostApplicationLifetime`, allowing the host to wait for completion before terminating.

Serilog is flushed before the application exits, ensuring no log entries are lost.

---

# How to Run

1. Ensure SQL Server is running (Docker recommended).
2. Update the connection string in `Program.cs`.
3. Run `dotnet ef database update` to apply migrations.
4. Run `dotnet run`.
5. Use Swagger UI at `/swagger` or use curl/Postman.