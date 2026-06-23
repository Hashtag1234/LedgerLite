using System;

namespace LedgerLite.Domain.Transactions;

// WHY: ProcessingResult is a simple immutable result type that communicates success or failure
// from domain operations without relying on exceptions for control flow.
public sealed record ProcessingResult
{
    public bool IsSuccess { get; init; }
    public string[] Errors { get; init; }

    public ProcessingResult(bool isSuccess, string[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    public static ProcessingResult Success() => new ProcessingResult(true, Array.Empty<string>());

    public static ProcessingResult Failure(params string[] errors) => new ProcessingResult(false, errors ?? Array.Empty<string>());
}
