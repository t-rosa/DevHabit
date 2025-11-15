namespace DevHabit.Api.Modules.Common;

public sealed record LinkResponse
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; init; }
}
