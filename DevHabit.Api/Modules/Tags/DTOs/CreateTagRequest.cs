namespace DevHabit.Api.Modules.Tags.DTOs;

public sealed record CreateTagRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
