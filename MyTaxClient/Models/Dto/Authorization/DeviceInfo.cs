namespace MyTaxClient.Models.Dto.Authorization;

public record DeviceInfo
{
    public required string AppVersion { get; init; }
    public required string SourceType { get; init; }
    public required string SourceDeviceId { get; init; }
    public required DeviceInfoMetaDetails MetaDetails { get; init; }
    
    public record DeviceInfoMetaDetails(string UserAgent);
}
