namespace MyTaxClient.Models.Dto.Authorization;

internal readonly record struct RefreshRequest(string RefreshToken, DeviceInfo DeviceInfo);