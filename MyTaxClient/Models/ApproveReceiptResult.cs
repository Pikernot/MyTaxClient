namespace MyTaxClient.Models;

public readonly record struct ApproveReceiptResult(
    bool IsSuccess,
    string? ReceiptUuid,
    string? Inn)
{
    public string? PrintUrl => ReceiptUuid is null || Inn is null
        ? null
        : $"https://lknpd.nalog.ru/api/v1/receipt/{Inn}/{ReceiptUuid}/print";
}