namespace DevHabit.Api.Modules.Habits.DTOs;

public sealed record CreateHabitRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyResponse Frequency { get; init; }
    public required TargetResponse Target { get; init; }
    public DateOnly? EndDate { get; init; }
    public MilestoneResponse? Milestone { get; init; }
}
