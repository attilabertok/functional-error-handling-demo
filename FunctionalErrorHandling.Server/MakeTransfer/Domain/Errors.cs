using FunctionalErrorHandling.Infrastructure;

namespace FunctionalErrorHandling.Server.MakeTransfer.Domain;

public static class Errors
{
    public sealed record InvalidBicError() : Error("The beneficiary's BIC/SWIFT code is invalid");

    public sealed record TransferDateIsPastError() : Error("Transfer date cannot be in the past");

    public sealed record TransferAmountIsInvalidError() : Error("Transferred amount must be positive");

    public sealed record UnexpectedError() : Error("An unexpected error occurred");

    public static Error InvalidBic => new InvalidBicError();

    public static Error TransferDateIsPast => new TransferDateIsPastError();

    public static Error Unexpected => new UnexpectedError();

    public static Error TransferAmountIsInvalid => new TransferAmountIsInvalidError();
}