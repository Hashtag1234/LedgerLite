using FluentValidation;
using LedgerLite.Api.Data;
using LedgerLite.Api.Validation;
using LedgerLite.Domain.Accounts;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LedgerLite.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/accounts")
            .WithName("Accounts");

        group.MapPost("/", CreateAccount)
            .WithName("CreateAccount")
            .Accepts<CreateAccountRequest>("application/json")
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("/", ListAccounts)
            .WithName("ListAccounts")
            .Produces<List<AccountResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetAccount)
            .WithName("GetAccount")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateAccount(
        CreateAccountRequest request,
        ApplicationDbContext dbContext,
        ValidationFilter<CreateAccountRequest>.AddEndpointFilter validator,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        var account = new Account(
            Guid.NewGuid(),
            request.Name,
            request.Type,
            new Money(request.InitialBalance, request.Currency));

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Account created: ID={AccountId}, Type={Type}, InitialBalance={Balance} {Currency}",
            account.Id,
            account.Type,
            account.Balance.Amount,
            account.Balance.Currency);

        return Results.Created(
            $"/api/v1/accounts/{account.Id}",
            new AccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type.ToString(),
                Balance = account.Balance.Amount,
                Currency = account.Balance.Currency
            });
    }

    private static async Task<IResult> ListAccounts(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var accounts = await dbContext.Accounts
            .OrderBy(a => a.Name)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type.ToString(),
                Balance = a.Balance.Amount,
                Currency = a.Balance.Currency
            })
            .ToListAsync(cancellationToken);

        return Results.Ok(accounts);
    }

    private static async Task<IResult> GetAccount(
        Guid id,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
        {
            return TypedResults.Problem(
                title: "Account not found.",
                detail: $"Account '{id}' does not exist.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.Ok(new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            Type = account.Type.ToString(),
            Balance = account.Balance.Amount,
            Currency = account.Balance.Currency
        });
    }
}

public record AccountResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
}
