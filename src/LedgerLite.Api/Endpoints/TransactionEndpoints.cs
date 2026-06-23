using FluentValidation;
using LedgerLite.Api.Data;
using LedgerLite.Api.Validation;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Transactions;
using LedgerLite.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace LedgerLite.Api.Endpoints;

// WHY: Endpoints are organized by feature. Extension methods on WebApplication
// allow clean registration of endpoint groups without a service class.
public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/transactions")
            .WithName("Transactions");

        // WHY: Use MapPost with request/response types for automatic OpenAPI generation.
        group.MapPost("/", CreateTransaction)
            .WithName("CreateTransaction")
            .Accepts<CreateTransactionRequest>("application/json")
            .Produces<TransactionResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetailsResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/", ListTransactions)
            .WithName("ListTransactions")
            .Produces<List<TransactionResponse>>(StatusCodes.Status200OK);
    }

    // WHY: Each endpoint accepts CancellationToken to support graceful shutdown.
    private static async Task<IResult> CreateTransaction(
        CreateTransactionRequest request,
        ApplicationDbContext dbContext,
        ValidationFilter<CreateTransactionRequest>.AddEndpointFilter validator,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        // WHY: Validate the request before processing.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new ProblemDetailsResponse
            {
                Status = 400,
                Title = "Validation failed.",
                Detail = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        try
        {
            // WHY: Load the account to verify it exists
            var account = await dbContext.Accounts.FirstOrDefaultAsync(
                a => a.Id == request.AccountId,
                cancellationToken);
            

            if (account is null)
            {
                return Results.NotFound(new ProblemDetailsResponse
                {
                    Status = 404,
                    Title = "Account not found.",
                    Detail = $"Account '{request.AccountId}' does not exist."
                });
            }

            // WHY: Construct domain objects from request. Amount sign is determined by Type.
            decimal signedAmount = request.Type == TransactionType.Income
                ? request.Amount
                : -request.Amount;

            var money = new Money(signedAmount, request.Currency);
            var category = new Category(request.Category);

            // WHY: Use the factory to create the transaction, enforcing domain rules.
            var transaction = TransactionFactory.Create(
                request.Type,
                Guid.NewGuid(),
                account.Id,
                money,
                category,
                DateTimeOffset.UtcNow,
                request.Description);

            // WHY: EF Core tracks and persists the transaction.
            dbContext.Transactions.Add(transaction);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Transaction created: ID={TransactionId}, Type={Type}, Amount={Amount} {Currency}",
                transaction.Id, transaction.Type, transaction.Amount.Amount, transaction.Amount.Currency);

            return Results.Created(
                $"/api/v1/transactions/{transaction.Id}",
                new TransactionResponse
                {
                    Id = transaction.Id,
                    AccountId = transaction.AccountId,
                    Type = transaction.Type.ToString(),
                    Amount = transaction.Amount.Amount,
                    Currency = transaction.Amount.Currency,
                    Category = transaction.Category.Name,
                    Description = transaction.Description,
                    Timestamp = transaction.Timestamp
                });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Invalid transaction: {Message}", ex.Message);
            return Results.BadRequest(new ProblemDetailsResponse
            {
                Status = 400,
                Title = "Invalid transaction.",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating transaction");
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> ListTransactions(
        Guid? accountId,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Transactions.AsQueryable();

        if (accountId.HasValue)
        {
            query = query.Where(t => t.AccountId == accountId.Value);
        }

        var transactions = await query
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Type = t.Type.ToString(),
                Amount = t.Amount.Amount,
                Currency = t.Amount.Currency,
                Category = t.Category.Name,
                Description = t.Description,
                Timestamp = t.Timestamp
            })
            .ToListAsync(cancellationToken);

        return Results.Ok(transactions);
    }
}

public record TransactionResponse
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; }
}

public class ProblemDetailsResponse
{
    public int Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
}
