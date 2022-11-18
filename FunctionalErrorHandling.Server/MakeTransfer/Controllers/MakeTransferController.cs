using FunctionalErrorHandling.Infrastructure;
using FunctionalErrorHandling.Server.MakeTransfer.Domain;
using FunctionalErrorHandling.Server.MakeTransfer.Dtos;
using FunctionalErrorHandling.Server.MakeTransfer.Validators;
using Microsoft.AspNetCore.Mvc;

using static FunctionalErrorHandling.Infrastructure.F;

using Unit = System.ValueTuple;

namespace FunctionalErrorHandling.Server.MakeTransfer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MakeTransferController : ControllerBase
    {
        private readonly ILogger<MakeTransferController> logger;
        private readonly BicValidator bicValidator;
        private readonly DateValidator dateValidator;
        private readonly AmountValidator amountValidator;
        private readonly Random rnd;

        public MakeTransferController(
            ILogger<MakeTransferController> logger,
            BicValidator bicValidator,
            DateValidator dateValidator,
            AmountValidator amountValidator)
        {
            this.logger = logger;
            this.bicValidator = bicValidator;
            this.dateValidator = dateValidator;
            this.amountValidator = amountValidator;
            rnd = new Random();
        }

        [HttpPost, Route("api/transfers/action-result")]
        public IActionResult MakeTransfer(MakeTransferRequest request) => Handle(request).Match(
            Invalid: BadRequest,
            Valid: result => result.Match(
                Exception: OnFaulted,
                Success: _ => Ok()));

        [HttpPost, Route("api/transfers/dto")]
        public ResultDto<Unit> MakeTransferDto(MakeTransferRequest request) =>
            Handle(request).ToResult(HandleException);

        private IActionResult OnFaulted(Exception ex)
        {
            logger.LogError(ex, "Could not save transfer");

            return StatusCode(500, Errors.Unexpected);
        }

        private void HandleException(Exception ex)
        {
            logger.LogError(ex, "Could not save transfer");
        }

        private Validation<Exceptional<Unit>> Handle(MakeTransferRequest request) =>
            Validate(request)
            .Map(Save);

        private Validation<MakeTransferRequest> Validate(MakeTransferRequest request) =>
            bicValidator.Validate(request)
                .Bind(dateValidator.Validate)
                .Bind(amountValidator.Validate);

        private Exceptional<Unit> Save(MakeTransferRequest request)
        {
            try
            {
                var result = rnd.Next(0, 3);
                if (result % 3 == 0)
                {
                    throw new Exception("Error when saving transfer");
                }

                logger.LogInformation("Successfully saved a transfer ${Amount} to {Bic} to be completed on {Date}", request.Amount, request.Bic, request.Date);
            }
            catch (Exception ex)
            {
                return ex;
            }

            return Unit();
        }

    }
}
