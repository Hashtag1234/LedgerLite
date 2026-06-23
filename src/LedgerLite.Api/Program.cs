using FluentValidation;
using Serilog;
using LedgerLite.Api.Data;
using LedgerLite.Api.Middleware;
using LedgerLite.Api.Validation;
using LedgerLite.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// WHY: Serilog replaces default logging with structured logging to JSON, enabling
// better observability in production (Phase 4: OpenTelemetry integration).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddOpenApi();

// WHY: DbContext with SQLite for Phase 1. Phase 5 switches to Azure SQL via connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=ledgerlite.db")
);

// WHY: FluentValidation integrates validators with DI.
// Each validator is auto-discovered and registered.
builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();

// WHY: Allow endpoints to inject `ValidationFilter<T>.AddEndpointFilter`.
// Register the open-generic nested type so DI can resolve the closed generic
// wrapper which forwards to FluentValidation's `IValidator<T>`.
builder.Services.AddTransient(typeof(ValidationFilter<>.AddEndpointFilter));

// WHY: Add logging and health checks.
builder.Services.AddLogging();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();


app.UseHttpsRedirection();

// WHY: In Phase 1, we skip database seeding and migrations.
// Phase 1.5 will add EF Core migrations and proper schema management.

// WHY: Map all endpoints for the domain.
app.MapTransactionEndpoints();

// WHY: Health check endpoint for monitoring and orchestrators.
app.MapHealthChecks("/health");

Log.Information("Starting LedgerLite API (Phase 1)...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
