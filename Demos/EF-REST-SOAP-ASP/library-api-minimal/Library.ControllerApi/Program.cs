using Library.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<LibraryDbContext>(options => options.UseSqlServer(conn_string));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger stuff
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
