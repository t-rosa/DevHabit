using DevHabit.Api.Modules.Common;

namespace DevHabit.Api.Modules.Tags.DTOs;

public sealed record TagsCollectionResponse : ICollectionResponse<TagResponse>
{
    public List<TagResponse> Items { get; init; }
}
