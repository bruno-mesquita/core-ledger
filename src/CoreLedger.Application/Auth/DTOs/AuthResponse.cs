namespace CoreLedger.Application.Auth.DTOs;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string Email,
    string Role);
