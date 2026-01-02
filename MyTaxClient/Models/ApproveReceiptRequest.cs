namespace MyTaxClient.Models;

public record ApproveReceiptRequest
{
    public required IEnumerable<Service> Services { get; set; }
    public required DateTimeOffset PaymentTime { get; set; }
}