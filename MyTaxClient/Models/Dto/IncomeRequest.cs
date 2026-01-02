namespace MyTaxClient.Models.Dto;

internal record IncomeRequest
{
    internal string PaymentType { get; init; } = "CASH";
    internal bool IgnoreMaxTotalIncomeRestriction { get; init; } = false;
    internal IncomeRequestClientInfo Client { get; init; } = new();
    internal required string OperationTime { get; init; }
    internal required string RequestTime { get; init; }
    internal required List<Service> Services { get; init; }
    internal decimal TotalAmount => Services.Sum(x => x.Amount);
    
    internal record IncomeRequestClientInfo(
        string? ContactPhone = null,
        string? DisplayName = null,
        string? Inn = null,
        string IncomeType = "FROM_INDIVIDUAL"
    );
}