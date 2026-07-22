using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Serilog;

using Library.ControllerApi.Middleware;
using Library.ControllerApi.Services;
using Library.ControllerApi.Mapping;
using Library.ControllerApi.Filters;
using Library.Data.Entities;
using Library.Data;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Write to console, and write to a file - starting a new file each day.
    .WriteTo.File("logs/fulfillment-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// CORS (Cross Origin Resource Sharing)
const string SpaCorsPolicy = "spa";
builder.Services.AddCors(o => o.AddPolicy(SpaCorsPolicy, 
    p => p.WithOrigins("http://localhost:5500", "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// JWT stuff
var jwtKey = builder.Configuration["Jwt:Key"]!;
var adminPass = builder.Configuration["AdminPass"]!;
const string jwtIssuer = "library-fulfillment";
const string jwtAudience = "library-fulfillment-clients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = jwtIssuer,
        ValidateAudience = true, ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHttpClient<ISupplierClient, SupplierClient>(c => c.BaseAddress = new Uri("https://dummyjson.com"));

// DbContext
var conn_string = "Server=localhost,1433;Database=LibraryMinimalDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";

builder.Services.AddDbContextFactory<LibraryDbContext>(o => o.UseSqlServer(conn_string));

// Filter apply to all controllers
builder.Services.AddControllers(o => o.Filters.Add<TimingFilter>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();


var app = builder.Build();

// Seeding admins
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    if(!db.Users.Any(u => u.Role == UserRoles.Admin))
    {
        var hasher = new PasswordHasher<User>();
        var admin = new User { UserName = "jon", Role = UserRoles.Admin };
        admin.PasswordHash = hasher.HashPassword(admin, adminPass);

        db.Users.Add(admin);
        db.SaveChanges();
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger stuff
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (ctx, next) =>
{
     var sw = System.Diagnostics.Stopwatch.StartNew();
     await next(ctx);
     sw.Stop();
     Log.Information("{method} {path} -> {statuscode} in {elapsed} ms",
        ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
});

app.UseResponseCaching();

app.UseCors(SpaCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
