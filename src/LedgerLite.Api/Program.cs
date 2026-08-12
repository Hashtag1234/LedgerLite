using FluentValidation;
using Serilog;
using LedgerLite.Api.Data;
using LedgerLite.Api.Middleware;
using LedgerLite.Api.Validation;
using LedgerLite.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// WHY: Serilog replaces default logging with structured JSON output, enabling
// better observability in production (Phase 4: OpenTelemetry integration).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// WHY: DbContext uses SQLite in Phase 1. Phase 5 will switch to Azure SQL via configuration.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=ledgerlite.db")
);

// WHY: FluentValidation integrates validators with DI.
// Validators are auto-discovered and registered.
builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();

// WHY: Allow endpoints to inject `ValidationFilter<T>.AddEndpointFilter`.
// Register the open-generic nested type so DI can resolve the closed generic
// wrapper that forwards to FluentValidation's `IValidator<T>`.
builder.Services.AddTransient(typeof(ValidationFilter<>.AddEndpointFilter));

// WHY: Add logging and health checks.
builder.Services.AddLogging();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.10
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();


app.UseHttpsRedirection();

// WHY: In Phase 1, we skip database seeding and migrations.
// Phase 1.5 will add EF Core migrations and proper schema management.
app.UseCors("AngularDev");
// WHY: Map all endpoints for the domain.
app.MapTransactionEndpoints();
app.MapAccountEndpoints();

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
