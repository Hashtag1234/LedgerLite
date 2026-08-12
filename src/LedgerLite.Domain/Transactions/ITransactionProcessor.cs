using System.Threading;
using System.Threading.Tasks;

namespace LedgerLite.Domain.Transactions;

// WHY: Defines the contract for transaction processing. The async method uses a CancellationToken
// to support cooperative cancellation and allow implementations to perform I/O or long-running work.
public interface ITransactionProcessor
{
    Task<ProcessingResult> ProcessAsync(Transaction transaction, CancellationToken cancellationToken);
}
