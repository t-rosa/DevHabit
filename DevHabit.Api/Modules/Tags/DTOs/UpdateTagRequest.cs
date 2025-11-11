namespace DevHabit.Api.Modules.Tags.DTOs;

public sealed record UpdateTagRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
