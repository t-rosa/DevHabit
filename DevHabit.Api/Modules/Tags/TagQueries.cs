using System.Linq.Expressions;
using DevHabit.Api.Modules.Tags.DTOs;

namespace DevHabit.Api.Modules.Tags;

internal static class TagQueries
{
    public static Expression<Func<Tag, TagResponse>> ProjectToResponse()
    {
        return t => new TagResponse
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc
        };
    }
}
