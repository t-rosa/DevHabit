using System.Linq.Expressions;
using DevHabit.Api.Database;
using DevHabit.Api.Modules.Habits.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Modules.Habits;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionResponse>> GetHabits()
    {
        List<HabitResponse> habits = await db
            .Habits
            .Select(HabitQueries.ProjectToResponse())
            .ToListAsync();

        var habitsCollectionResponse = new HabitsCollectionResponse
        {
            Items = habits
        };

        return Ok(habitsCollectionResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitWithTagsResponse>> GetHabit(string id)
    {
        HabitWithTagsResponse? habit = await db
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToResponseWithTags())
            .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }

        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request, IValidator<CreateHabitRequest> validator)
    {
        await validator.ValidateAndThrowAsync(request);

        Habit habit = request.ToEntity();

        db.Habits.Add(habit);

        await db.SaveChangesAsync();

        var response = habit.ToHabitResponse();

        return CreatedAtAction(nameof(GetHabit), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitRequest request)
    {
        Habit? habit = await db.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        habit.UpdateFromRequest(request);

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitResponse> patchDocument)
    {
        Habit? habit = await db.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        var response = habit.ToHabitResponse();
        patchDocument.ApplyTo(response);

        if (!TryValidateModel(ModelState))
        {
            return ValidationProblem(ModelState);
        }

        habit.Name = response.Name;
        habit.Description = response.Description;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await db.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit == null)
        {
            return NotFound();
        }

        db.Habits.Remove(habit);

        await db.SaveChangesAsync();

        return NoContent();
    }
};
