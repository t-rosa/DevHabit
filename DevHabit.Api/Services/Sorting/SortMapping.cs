namespace DevHabit.Api.Services.Sorting;

public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);

#pragma warning disable S2326 // Unused type parameters should be removed
public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
#pragma warning restore S2326 // Unused type parameters should be removed
{
    public required SortMapping[] Mappings { get; init; }
}
