namespace aprendendo_api.Application.DTOs;

public record UpdateUserRequest(string? Email, string? Password, string? Role);
