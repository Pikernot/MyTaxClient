namespace MyTaxClient.Models;

public readonly record struct CancelReceiptResult(
    bool IsSuccess,
    string? MyTaxResponseText);