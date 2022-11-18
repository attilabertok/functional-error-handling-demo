using FunctionalErrorHandling.Infrastructure;
using FunctionalErrorHandling.Server.MakeTransfer.Domain;
using FunctionalErrorHandling.Server.MakeTransfer.Dtos;

namespace FunctionalErrorHandling.Server.MakeTransfer.Validators;

public class DateValidator
{
    public Validation<MakeTransferRequest> Validate(MakeTransferRequest request) =>
        request.Date.Date >= DateTime.Today
            ? request
            : Errors.TransferDateIsPast;
}
