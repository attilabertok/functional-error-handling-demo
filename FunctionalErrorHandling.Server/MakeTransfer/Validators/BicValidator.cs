using System.Text.RegularExpressions;
using FunctionalErrorHandling.Infrastructure;
using FunctionalErrorHandling.Server.MakeTransfer.Domain;
using FunctionalErrorHandling.Server.MakeTransfer.Dtos;

namespace FunctionalErrorHandling.Server.MakeTransfer.Validators;

public class BicValidator
{
    private readonly Regex bicRegex = new("[A-Z]{11}");

    public Validation<MakeTransferRequest> Validate(MakeTransferRequest request) =>
        bicRegex.IsMatch(request.Bic)
            ? request
            : Errors.InvalidBic;
}
