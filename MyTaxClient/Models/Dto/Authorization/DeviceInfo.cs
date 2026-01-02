namespace MyTaxClient.Models.Dto.Authorization;

internal record DeviceInfo
{
    internal required string AppVersion { get; init; }
    internal required string SourceType { get; init; }
    internal required string SourceDeviceId { get; init; }
    internal required DeviceInfoMetaDetails MetaDetails { get; init; }
    
    internal record DeviceInfoMetaDetails(string UserAgent);
}
