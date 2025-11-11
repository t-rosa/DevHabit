namespace DevHabit.Api.Modules.HabitTags.DTOs;

public sealed record UpsertHabitTagRequest
{
    public required List<string> TagIds { get; init; }
}
