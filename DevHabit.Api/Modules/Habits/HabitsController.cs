using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using DevHabit.Api.Database;
using DevHabit.Api.Modules.Common;
using DevHabit.Api.Modules.Habits.DTOs;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
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
    public async Task<IActionResult> GetHabits([FromQuery] HabitsQueryParameters query, SortMappingProvider sortMappingProvider, DataShapingService dataShapingService)
    {
        if (!sortMappingProvider.ValidateMappings<HabitResponse, Habit>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'"
            );
        }

        if (!dataShapingService.Validate<HabitResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{query.Fields}'"
            );
        }

        query.Search ??= query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<HabitResponse, Habit>();

#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
        IQueryable<HabitResponse> habitsQuery = db
            .Habits
            .Where(h => query.Search == null ||
                h.Name.ToLower().Contains(query.Search) ||
                h.Description != null && h.Description.ToLower().Contains(query.Search))
            .Where(h => query.Type == null || h.Type == query.Type)
            .Where(h => query.Status == null || h.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToResponse());
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons

        int totalCount = await habitsQuery.CountAsync();

        List<HabitResponse> habits = await habitsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var response = new PaginationResult<ExpandoObject>
        {
            Items = dataShapingService.ShapeCollectionData(habits, query.Fields),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabit(string id, string? fields, DataShapingService dataShapingService)
    {
        if (!dataShapingService.Validate<HabitWithTagsResponse>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{fields}'"
            );
        }

        HabitWithTagsResponse? habit = await db
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToResponseWithTags())
            .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }

        dataShapingService.ShapeData(habit, fields);

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
