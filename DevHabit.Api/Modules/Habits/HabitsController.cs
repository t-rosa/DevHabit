using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net.Mime;
using DevHabit.Api.Database;
using DevHabit.Api.Modules.Common;
using DevHabit.Api.Modules.Habits.DTOs;
using DevHabit.Api.Modules.HabitTags;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Modules.Habits;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext db, LinkService linkService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHabits(
        [FromQuery] HabitsQueryParameters query,
        SortMappingProvider sortMappingProvider,
        DataShapingService dataShapingService)
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
            Items = dataShapingService.ShapeCollectionData(
                habits,
                query.Fields,
                query.IncludeLinks ? h => CreateLinksForHabit(h.Id, query.Fields) : null),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };

        if (query.IncludeLinks)
        {
            response.Links = CreateLinksForHabits(query, response.HasNextPage, response.HasPreviousPage);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabit(
        string id,
        [FromQuery] HabitQueryParameters query,
        DataShapingService dataShapingService)
    {
        if (!dataShapingService.Validate<HabitWithTagsResponse>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{query.Fields}'"
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

        ExpandoObject shapedHabit = dataShapingService.ShapeData(habit, query.Fields);

        if (query.IncludeLinks)
        {
            shapedHabit.TryAdd("links", CreateLinksForHabit(id, query.Fields));
        }

        return Ok(shapedHabit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request, IValidator<CreateHabitRequest> validator)
    {
        await validator.ValidateAndThrowAsync(request);

        Habit habit = request.ToEntity();

        db.Habits.Add(habit);

        await db.SaveChangesAsync();

        var response = habit.ToHabitResponse();

        response.Links = CreateLinksForHabit(habit.Id, null);

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

    private List<LinkResponse> CreateLinksForHabits(
        HabitsQueryParameters query,
        bool hasNextPage,
        bool hasPreviousPage
    )
    {
        List<LinkResponse> links = [
            linkService.Create(nameof(GetHabits), "self", HttpMethods.Get, new
            {
                page = query.Page,
                pageSize = query.PageSize,
                fields = query.Fields,
                q = query.Search,
                sort = query.Sort,
                type = query.Type,
                status = query.Status
            }),

            linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(nameof(GetHabits), "next-page", HttpMethods.Get, new
                {
                    page = query.Page + 1,
                    pageSize = query.PageSize,
                    fields = query.Fields,
                    q = query.Search,
                    sort = query.Sort,
                    type = query.Type,
                    status = query.Status
                })
            );
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(nameof(GetHabits), "previous-page", HttpMethods.Get, new
                {
                    page = query.Page - 1,
                    pageSize = query.PageSize,
                    fields = query.Fields,
                    q = query.Search,
                    sort = query.Sort,
                    type = query.Type,
                    status = query.Status
                })
            );
        }

        return links;
    }

    private List<LinkResponse> CreateLinksForHabit(string id, string? fields)
    {
        List<LinkResponse> links = [
            linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(UpdateHabit), "patch", HttpMethods.Patch, new { id }),
            linkService.Create(nameof(UpdateHabit), "delete", HttpMethods.Delete, new { id }),
            linkService.Create(
                nameof(HabitTagsController.UpsertHabitTags),
                "upsert-tags",
                HttpMethods.Put,
                new { habitId = id },
                HabitTagsController.Name
            ),
        ];

        return links;
    }
};
