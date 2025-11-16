namespace DevHabit.Api.Modules.Auth.DTOs;

public sealed record RegisterRequest
{
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
}
