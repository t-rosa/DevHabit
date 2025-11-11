namespace DevHabit.Api.Modules.Habits.DTOs;

public sealed record UpdateHabitRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyResponse Frequency { get; init; }
    public required TargetResponse Target { get; init; }
    public DateOnly? EndDate { get; init; }
    public UpdateMilestoneRequest? Milestone { get; init; }
}

public sealed record UpdateMilestoneRequest
{
    public required int Target { get; init; }
}
