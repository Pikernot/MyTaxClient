namespace MyTaxClient.Models.Dto;

public record CancelRequest(
    string ReceiptUuid,
    string OperationTime,
    string RequestTime,
    string CancelReason);