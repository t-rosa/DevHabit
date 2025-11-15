using DevHabit.Api.Database;
using DevHabit.Api.Modules.Habits;
using DevHabit.Api.Modules.HabitTags.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Modules.HabitTags;

[ApiController]
[Route("habits/{habitId}/tags")]
public sealed class HabitTagsController(ApplicationDbContext db) : ControllerBase
{
    public static readonly string Name = nameof(HabitTagsController).Replace("Controller", string.Empty);

    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagRequest request)
    {
        Habit? habit = await db.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId);

        if (habit == null)
        {
            return NotFound();
        }

        var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();
        if (currentTagIds.SetEquals(request.TagIds))
        {
            return NoContent();
        }

        List<string> existingTagIds = await db
            .Tags
            .Where(t => request.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        if (existingTagIds.Count != request.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid");
        }

        habit.HabitTags.RemoveAll(ht => !request.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = request.TagIds.Except(currentTagIds).ToArray();
        habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
        {
            HabitId = habitId,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow
        }));

        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteHabitTag(string habitId, string tagId)
    {
        HabitTag? habitTag = await db
            .HabitTags
            .Where(ht => ht.HabitId == habitId)
            .Where(ht => ht.TagId == tagId)
            .FirstOrDefaultAsync();

        if (habitTag == null)
        {
            return NotFound();
        }

        db.HabitTags.Remove(habitTag);

        await db.SaveChangesAsync();

        return NoContent();
    }
}
