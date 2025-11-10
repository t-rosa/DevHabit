using DevHabit.Api.Database;
using DevHabit.Api.Modules.Habits.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Modules.Habits;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionResponse>> GetHabits()
    {
        List<HabitResponse> habits = await dbContext
            .Habits
            .Select(h => new HabitResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyResponse
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetResponse
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone == null ? null : new MilestoneResponse
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            }).ToListAsync();

        var habitsCollectionResponse = new HabitsCollectionResponse
        {
            Items = habits
        };

        return Ok(habitsCollectionResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitResponse>> GetHabit(string id)
    {
        HabitResponse? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(h => new HabitResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyResponse
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetResponse
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone == null ? null : new MilestoneResponse
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            }).FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }

        return Ok(habit);
    }
};
