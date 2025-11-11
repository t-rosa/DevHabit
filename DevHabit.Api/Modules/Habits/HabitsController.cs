using System.Linq.Expressions;
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
            .Select(HabitQueries.ProjectToHabitResponse())
            .ToListAsync();

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
            .Select(HabitQueries.ProjectToHabitResponse())
            .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }

        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request)
    {
        Habit habit = request.ToEntity();

        dbContext.Habits.Add(habit);

        await dbContext.SaveChangesAsync();

        var response = habit.ToHabitResponse();

        return CreatedAtAction(nameof(GetHabit), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitRequest request)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        habit.UpdateFromRequest(request);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
};
