using DevHabit.Api.Modules.Common;

namespace DevHabit.Api.Modules.Habits.DTOs;

public sealed record HabitResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyResponse Frequency { get; init; }
    public required TargetResponse Target { get; init; }
    public required HabitStatus Status { get; init; }
    public required bool IsArchived { get; init; }
    public DateOnly? EndDate { get; init; }
    public MilestoneResponse? Milestone { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedAtUtc { get; init; }
    public List<LinkResponse> Links { get; set; }
}

public sealed record FrequencyResponse
{
    public required FrequencyType Type { get; init; }
    public required int TimesPerPeriod { get; init; }
}

public sealed record MilestoneResponse
{
    public required int Target { get; init; }
    public required int Current { get; init; }
}

public sealed record TargetResponse
{
    public required int Value { get; init; }
    public required string Unit { get; init; }
}
