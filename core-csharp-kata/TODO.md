## What the App Must Do (User Stories)

### Collections & ordering

- [x] **Add and list.** *As a user, I can add an entity and list everything currently held; the list grows and
  shrinks as I add or remove — no fixed cap.*
  - [x] Add Items
  - [x] Remove Items
- [x] **Undo my last action.** *As a user, I can undo my most recent change, and the one before it is untouched.*
  - [x] Undo Action
- [ ] **Serve in arrival order.** *As a user, I can process pending requests (restock requests, a teller line, a
  play queue) in the exact order they arrived.*
  - Accept: the **first** request added is served first.
- [ ] **Reorder my working list.** *As a user, I can move an urgent entity to the front of a working list without
  rebuilding the list.*
  - Accept: an item sent to the front prints first on the next listing.
- [ ] **See a grid view.** *As a user, I can view a fixed rows×columns layout (aisle×shelf, lot×row, week×day).*
  - Accept: the app reports the grid's two dimensions and places items at `[row, col]`.

### Failing safely & a log trail

- [ ] **Clear errors, no crash.** *As a user, when I ask for an entity that doesn't exist, I get a clear message
  naming what was missing, and the app keeps running.*
  - Accept: a bad lookup does not crash; the error names the missing id; the menu returns.
- [x] **A trail of what happened.** *As an operator, I can read a running log of what the app did, tagged by
  severity (routine / warning / failure).*
  - Accept: a session shows at least one info, one warning, and one error line tied to real actions.

### Fast lookups & flexible browsing

- [x] **Instant lookup by key.** *As a user, I can pull up any entity immediately by its natural key (SKU,
  account number, VIN) without scanning the whole list.*
  - Accept: looking up a missing key reports "not found" cleanly rather than crashing.
- [x] **Distinct values, no duplicates.** *As a user, I can see each distinct attribute once (each supplier, each
  owner, each artist), however many entities share it.*
  - Accept: the distinct count is **less than** the entity count when duplicates exist.
- [x] **Browse everything in one pass.** *As a user, I can browse the whole collection with a single list
  command.*
  - Accept: one command walks every entity in order.
- [ ] **Search by my own condition.** *As a user, I can search for entities matching a condition I supply (price
  over X, balance under Y, genre = Z).*
  - Accept: the same search command returns different results for different conditions.

### Live data & input it can trust

- [ ] **Enrich from a live source.** *As a user, I can add or enrich an entity using real data fetched from a
  public online source, and the app stays responsive while it fetches.*
  - Accept: the fetched data shows up on the entity; the app does not freeze during the call.
- [ ] **Survive a network error.** *As a user, if the fetch fails the app reports it and keeps running instead
  of crashing.*
  - Accept: a failed fetch is logged and the command reports "nothing fetched"; the app does not crash.
- [x] **Reject bad input.** *As a user, the app refuses a malformed identifier before it saves anything.*
  - Accept: a wrongly-shaped id is rejected with a message; a well-shaped one is accepted.

## Techniques You Must Demonstrate

The finished app must use **each** of these **somewhere** — you decide which story each one serves.

- [x] `List<T>` · `Stack<T>` (LIFO) · `Queue<T>` (FIFO) · `LinkedList<T>` · multi-dimensional array `T[,]`
- [x] `enum` · `readonly struct` · a generic type you wrote yourself
- [x] custom exception (carries data) · `try`/`catch`/`finally`
- [x] one pattern: repository behind an interface **or** factory
- [x] Serilog structured logging (info / warning / error)
- [x] `Dictionary<K,V>` + `TryGetValue` · `HashSet<T>`
- [ ] `IEnumerable<T>` + `yield return` · lambda / `Predicate<T>` filter
- [x] expression-bodied member · `partial` **or** `sealed`
- [ ] shared `HttpClient` + `async`/`await` · `HttpRequestException` handling
- [ ] JSON deserialization (read the fields, build via your factory/constructor)
- [ ] `Regex` validation · one of: `out` param / nullable + lifted operator / pattern-matching `switch`


## Stretch (each group is required to pick 2 to implement.)

Pick atleast two (or come up with your own!). With three pairs of hands you have room to pick a couple — but the core (every ship criterion) comes first. Once every ship criterion passes you can:

- [ ] Add a **generic constraint** to your custom generic type (e.g. `where T : class`) and note in the PR why it
  fits or does not fit your element type.
- [ ] Add a **second `yield` method** that filters during iteration and confirm nothing runs until you `foreach`
  it (deferred execution).
- [ ] Add a **second Serilog sink** (e.g. `.WriteTo.File(...)`) and confirm the call sites did not change.
- [ ] Issue **two** HTTP requests with **`Task.WhenAll`** and confirm they overlap rather than run back-to-back.
- [ ] Add a **second design pattern** (e.g. ship both a repository *and* a factory) and name both in the PR.
- [ ] Add a short **`CONTRIBUTING.md`** noting your team's branch/PR/review convention.