using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LedgerLite.Domain.Accounts;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;

namespace LedgerLite.Domain.Transactions;

// WHY: A simple in-memory transaction processor demonstrates applying domain rules
// and returning a `ProcessingResult`, keeping Phase 0 free of external dependencies.
public sealed class SimpleTransactionProcessor : ITransactionProcessor
{
    private readonly IDictionary<Guid, Account> _accounts;

    public SimpleTransactionProcessor(IDictionary<Guid, Account> accounts)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
    }

    public Task<ProcessingResult> ProcessAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        if (!_accounts.TryGetValue(transaction.AccountId, out Account? account))
        {
            return Task.FromResult(ProcessingResult.Failure("Account not found"));
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Normalize amount to positive when applying deposit/withdraw
            Money amt = transaction.Amount.Amount < 0
                ? new Money(Math.Abs(transaction.Amount.Amount), transaction.Amount.Currency)
                : transaction.Amount;

            if (transaction.Type == TransactionType.Income)
            {
                account.Deposit(amt);
            }
            else // Expense
            {
                account.Withdraw(amt);
            }

            return Task.FromResult(ProcessingResult.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult(ProcessingResult.Failure(ex.Message));
        }
    }
}
