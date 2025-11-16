using System.Net.Http.Headers;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Modules.Common;

public record AcceptHeaderResponse
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    public bool IncludeLinks =>
        MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediaType) &&
        mediaType.MediaType == CustomMediaTypeNames.Application.HateoasJson;
}
