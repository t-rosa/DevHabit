namespace DevHabit.Api.Modules.Common;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
