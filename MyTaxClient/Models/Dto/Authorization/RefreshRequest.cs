namespace MyTaxClient.Models.Dto.Authorization;

public readonly record struct RefreshRequest(string RefreshToken, DeviceInfo DeviceInfo);