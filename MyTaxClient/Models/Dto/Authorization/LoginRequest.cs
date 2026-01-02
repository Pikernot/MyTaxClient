namespace MyTaxClient.Models.Dto.Authorization;

public readonly record struct LoginRequest(
    string Username,
    string Password,
    DeviceInfo DeviceInfo);