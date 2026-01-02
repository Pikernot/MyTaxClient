namespace MyTaxClient.Models;

public record CancelReceiptRequest
{
    /// <summary>
    /// UUID аннулируемого чека
    /// </summary>
    public required string ReceiptUuid { get; init; }
    /// <summary>
    /// Время аннулирования
    /// </summary>
    public required DateTimeOffset CancellationTime { get; init; }
    /// <summary>
    /// Причина отмены
    /// </summary>
    public string CancelReason { get; init; } = "Чек сформирован ошибочно";
}
