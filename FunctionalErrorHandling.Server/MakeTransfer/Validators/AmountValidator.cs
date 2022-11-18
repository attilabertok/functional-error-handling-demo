using FunctionalErrorHandling.Infrastructure;
using FunctionalErrorHandling.Server.MakeTransfer.Domain;
using FunctionalErrorHandling.Server.MakeTransfer.Dtos;

namespace FunctionalErrorHandling.Server.MakeTransfer.Validators;

public class AmountValidator
{
    public Validation<MakeTransferRequest> Validate(MakeTransferRequest request) =>
        request.Amount > 0
            ? request
            : Errors.TransferAmountIsInvalid;
}
