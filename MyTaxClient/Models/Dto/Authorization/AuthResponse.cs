namespace MyTaxClient.Models.Dto.Authorization;

public record AuthResponse
{
    public required string Token { get; init; }
    public required DateTimeOffset TokenExpireIn { get; init; }
    public required string RefreshToken { get; init; }
    public UserProfile? Profile { get; init; }
        
    public record UserProfile(
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