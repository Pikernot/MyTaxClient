namespace MyTaxClient.Models.Dto.Authorization;

internal readonly record struct LoginRequest(
    string Username,
    string Password,
    DeviceInfo DeviceInfo);