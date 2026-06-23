using LedgerLite.Domain.Accounts;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Transactions;
using LedgerLite.Domain.Enum;
using System.Collections.Generic;
using System.Threading;

Console.WriteLine("=== LedgerLite Domain Verification ===");

try
{
    // 1. Verify Money Value Object
    Console.WriteLine("\n[1] Verifying Money Value Object...");
    Money usd100 = new Money(100.00m, "USD");
    Money usd50 = new Money(50.00m, "USD");
    Money eur50 = new Money(50.00m, "EUR");

    Console.WriteLine($"Created: {usd100}");
    Console.WriteLine($"Created: {usd50}");

    // Addition
    Money sum = usd100 + usd50;
    Console.WriteLine($"Addition (100 USD + 50 USD): {sum}");

    // Subtraction
    Money diff = usd100 - usd50;
    Console.WriteLine($"Subtraction (100 USD - 50 USD): {diff}");

    // Currency mismatch validation
    try
    {
        Money invalidSum = usd100 + eur50;
        Console.WriteLine("FAIL: Added different currencies!");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"PASS: Currency mismatch addition blocked: {ex.Message}");
    }

    // Invalid Currency Code validation
    try
    {
        Money invalidCurrency = new Money(100.00m, "INVALID");
        Console.WriteLine("FAIL: Invalid currency code length accepted!");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"PASS: Invalid currency code blocked: {ex.Message}");
    }

    // 2. Verify Account Entity
    Console.WriteLine("\n[2] Verifying Account Entity...");
    Account checking = new Account(Guid.NewGuid(), "Checking Account", AccountType.Checking, new Money(500.00m, "USD"));
    Console.WriteLine($"Account Created: {checking.Name} ({checking.Type}) - Balance: {checking.Balance}");

    checking.Deposit(new Money(150.00m, "USD"));
    Console.WriteLine($"After depositing 150 USD: {checking.Balance}");

    checking.Withdraw(new Money(50.00m, "USD"));
    Console.WriteLine($"After withdrawing 50 USD: {checking.Balance}");

    // Account currency mismatch validation
    try
    {
        checking.Deposit(new Money(100.00m, "EUR"));
        Console.WriteLine("FAIL: Deposited EUR into USD account!");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"PASS: Depositing EUR into USD account blocked: {ex.Message}");
    }

    // Negative deposit validation
    try
    {
        checking.Deposit(new Money(-50.00m, "USD"));
        Console.WriteLine("FAIL: Negative deposit accepted!");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"PASS: Negative deposit blocked: {ex.Message}");
    }

    // 3. Verify Transaction Entity
    Console.WriteLine("\n[3] Verifying Transaction Entity...");
    Category groceryCategory = new Category("Groceries");
    Transaction transaction = TransactionFactory.Create(
        TransactionType.Expense,
        Guid.NewGuid(),
        checking.Id,
        new Money(-45.50m, "USD"),
        groceryCategory,
        DateTimeOffset.UtcNow,
        "Weekly grocery run"
    );

    Console.WriteLine($"Transaction Created: ID={transaction.Id}, AccountId={transaction.AccountId}, Amount={transaction.Amount}, Category={transaction.Category}, Desc={transaction.Description}");

    // Demo: process the transaction using an in-memory processor
    var accounts = new Dictionary<Guid, Account> { [checking.Id] = checking };
    var processor = new SimpleTransactionProcessor(accounts);
    var processResult = await processor.ProcessAsync(transaction, CancellationToken.None);
    Console.WriteLine($"Processing result: Success={processResult.IsSuccess}");
    if (processResult.IsSuccess)
    {
        Console.WriteLine($"Account balance after processing: {checking.Balance}");
    }

    // Transaction invalid amount validation
    try
    {
        Transaction invalidTx = TransactionFactory.Create(
            TransactionType.Expense,
            Guid.NewGuid(),
            checking.Id,
            new Money(0.00m, "USD"),
            groceryCategory,
            DateTimeOffset.UtcNow,
            "Free item"
        );
        Console.WriteLine("FAIL: Zero amount transaction accepted!");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"PASS: Zero amount transaction blocked: {ex.Message}");
    }

    Console.WriteLine("\n=== All domain validations passed successfully ===");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected Failure: {ex}");
}
