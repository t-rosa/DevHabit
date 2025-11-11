using DevHabit.Api.Modules.Tags.DTOs;

namespace DevHabit.Api.Modules.Tags;

internal static class TagMappings
{
    public static TagResponse ToTagResponse(this Tag tag)
    {
        return new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            CreatedAtUtc = tag.CreatedAtUtc,
            UpdatedAtUtc = tag.UpdatedAtUtc
        };
    }

    public static Tag ToEntity(this CreateTagRequest request)
    {
        return new Tag
        {
            Id = $"t_{Guid.CreateVersion7()}",
            Name = request.Name,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public static void UpdateFromRequest(this Tag tag, UpdateTagRequest request)
    {
        tag.Name = request.Name;
        tag.Description = request.Description;
        tag.UpdatedAtUtc = DateTime.UtcNow;
    }
}
