using DevHabit.Api.Database;
using DevHabit.Api.Modules.Common;
using DevHabit.Api.Modules.Tags.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Modules.Tags;

[ApiController]
[Route("tags")]
public sealed class TagsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TagsCollectionResponse>> GetTags()
    {
        List<TagResponse> tags = await db
            .Tags
            .Select(TagQueries.ProjectToResponse())
            .ToListAsync();

        var response = new PaginationResult<TagResponse>
        {
            Items = tags
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagResponse>> GetTag(string id)
    {
        TagResponse? tag = await db
            .Tags
            .Where(t => t.Id == id)
            .Select(TagQueries.ProjectToResponse())
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost()]
    public async Task<ActionResult<TagResponse>> CreateTag(CreateTagRequest request, IValidator<CreateTagRequest> validator)
    {
        await validator.ValidateAndThrowAsync(request);

        Tag tag = request.ToEntity();

        // Race condition
        if (await db.Tags.AnyAsync(t => t.Name == tag.Name))
        {
            return Problem(detail: $"The tag '{tag.Name}' already exists", statusCode: StatusCodes.Status409Conflict);
        }

        db.Tags.Add(tag);

        // Race condition (throw if the name is not unique)
        await db.SaveChangesAsync();

        var response = tag.ToTagResponse();

        return CreatedAtAction(nameof(GetTag), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTag(string id, UpdateTagRequest request)
    {
        Tag? tag = await db
            .Tags
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null)
        {
            return NotFound();
        }

        tag.UpdateFromRequest(request);

        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        Tag? tag = await db
            .Tags
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null)
        {
            return NotFound();
        }

        db.Tags.Remove(tag);

        await db.SaveChangesAsync();

        return NoContent();
    }
}
