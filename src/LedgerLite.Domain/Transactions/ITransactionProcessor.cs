using System.Threading;
using System.Threading.Tasks;

namespace LedgerLite.Domain.Transactions;

// WHY: Defines the contract for transaction processing. Use an async method with a CancellationToken
// to support cooperative cancellation and to allow implementations to perform I/O or long-running work.
public interface ITransactionProcessor
{
    Task<ProcessingResult> ProcessAsync(Transaction transaction, CancellationToken cancellationToken);
}
