namespace FunctionalErrorHandling.Server.MakeTransfer.Dtos;

public class MakeTransferRequest
{
    public decimal Amount { get; set; }

    public string Bic { get; set; }

    public DateTime Date { get; set; }
}
