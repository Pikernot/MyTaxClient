namespace MyTaxClient.Models.Dto.Authorization;

internal record AuthResponse
{
    internal required string Token { get; init; }
    internal required DateTimeOffset TokenExpireIn { get; init; }
    internal required string RefreshToken { get; init; }
    internal UserProfile? Profile { get; init; }
        
    internal record UserProfile(
        string? LastName,
        long? Id,
        string? DisplayName,
        string? MiddleName,
        string? Email,
        string? Phone,
        string? Inn,
        string? Snils,
        bool? AvatarExists,
        string? InitialRegistrationDate,
        string? RegistrationDate,
        string? FirstReceiptRegisterTime,
        string? FirstReceiptCancelTime,
        bool? HideCancelledReceipt,
        string? RegisterAvailable,
        string? Status,
        bool? RestrictedMode,
        string? PfrUrl,
        string? Login);
}