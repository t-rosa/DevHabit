namespace DevHabit.Api.Modules.Tags.DTOs;

public sealed record TagsCollectionResponse
{
    public List<TagResponse> Items { get; init; }
}

public sealed record TagResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
