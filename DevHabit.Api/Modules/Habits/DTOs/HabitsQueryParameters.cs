using DevHabit.Api.Modules.Common;
using DevHabit.Api.Modules.Habits;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Modules.Habits.DTOs;

public sealed record HabitsQueryParameters : AcceptHeaderResponse
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    public HabitType? Type { get; init; }
    public HabitStatus? Status { get; init; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}


public sealed record HabitQueryParameters : AcceptHeaderResponse
{
    public string? Fields { get; init; }
}
