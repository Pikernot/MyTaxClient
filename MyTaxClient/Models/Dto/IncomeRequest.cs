namespace MyTaxClient.Models.Dto;

public record IncomeRequest
{
    public string PaymentType { get; init; } = "CASH";
    public bool IgnoreMaxTotalIncomeRestriction { get; init; } = false;
    public IncomeRequestClientInfo Client { get; init; } = new();
    public required string OperationTime { get; init; }
    public required string RequestTime { get; init; }
    public required List<Service> Services { get; init; }
    public decimal TotalAmount => Services.Sum(x => x.Amount);
    
    public record IncomeRequestClientInfo(
        string? ContactPhone = null,
        string? DisplayName = null,
        string? Inn = null,
        string IncomeType = "FROM_INDIVIDUAL"
    );
}