using DevHabit.Api.Modules.Habits.DTOs;
using DevHabit.Api.Services.Sorting;

namespace DevHabit.Api.Modules.Habits;

internal static class HabitMappings
{
    public static readonly SortMappingDefinition<HabitResponse, Habit> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(HabitResponse.Name), nameof(Habit.Name)),
            new SortMapping(nameof(HabitResponse.Description), nameof(Habit.Description)),
            new SortMapping(nameof(HabitResponse.Type), nameof(Habit.Type)),
            new SortMapping(
                $"{nameof(HabitResponse.Frequency)}.{nameof(FrequencyResponse.Type)}",
                $"{nameof(Habit.Frequency)}.{nameof(Frequency.Type)}"
            ),
            new SortMapping(
                $"{nameof(HabitResponse.Frequency)}.{nameof(FrequencyResponse.TimesPerPeriod)}",
                $"{nameof(Habit.Frequency)}.{nameof(Frequency.TimesPerPeriod)}"
            ),
            new SortMapping(
                $"{nameof(HabitResponse.Target)}.{nameof(TargetResponse.Value)}",
                $"{nameof(Habit.Target)}.{nameof(Target.Value)}"
            ),
            new SortMapping(
                $"{nameof(HabitResponse.Target)}.{nameof(TargetResponse.Unit)}",
                $"{nameof(Habit.Target)}.{nameof(Target.Unit)}"
            ),
            new SortMapping(nameof(HabitResponse.Status), nameof(Habit.Status)),
            new SortMapping(nameof(HabitResponse.IsArchived), nameof(Habit.IsArchived)),
            new SortMapping(nameof(HabitResponse.EndDate), nameof(Habit.EndDate)),
            new SortMapping(nameof(HabitResponse.CreatedAtUtc), nameof(Habit.CreatedAtUtc)),
            new SortMapping(nameof(HabitResponse.UpdatedAtUtc), nameof(Habit.UpdatedAtUtc)),
            new SortMapping(nameof(HabitResponse.LastCompletedAtUtc), nameof(Habit.LastCompletedAtUtc)),
        ]
    };

    public static HabitResponse ToHabitResponse(this Habit habit)
    {
        return new HabitResponse
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Type = habit.Type,
            Frequency = new FrequencyResponse
            {
                Type = habit.Frequency.Type,
                TimesPerPeriod = habit.Frequency.TimesPerPeriod
            },
            Target = new TargetResponse
            {
                Value = habit.Target.Value,
                Unit = habit.Target.Unit
            },
            Status = habit.Status,
            IsArchived = habit.IsArchived,
            EndDate = habit.EndDate,
            Milestone = habit.Milestone == null ? null : new MilestoneResponse
            {
                Target = habit.Milestone.Target,
                Current = habit.Milestone.Current
            },
            CreatedAtUtc = habit.CreatedAtUtc,
            UpdatedAtUtc = habit.UpdatedAtUtc,
            LastCompletedAtUtc = habit.LastCompletedAtUtc
        };
    }

    public static Habit ToEntity(this CreateHabitRequest request)
    {
        Habit habit = new()
        {
            Id = $"h_{Guid.CreateVersion7()}",
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            Frequency = new Frequency
            {
                Type = request.Frequency.Type,
                TimesPerPeriod = request.Frequency.TimesPerPeriod
            },
            Target = new Target
            {
                Value = request.Target.Value,
                Unit = request.Target.Unit
            },
            Status = HabitStatus.Ongoing,
            IsArchived = false,
            EndDate = request.EndDate,
            Milestone = request.Milestone != null ? new Milestone
            {
                Target = request.Milestone.Target,
                Current = 0
            } : null,
            CreatedAtUtc = DateTime.UtcNow
        };

        return habit;
    }

    public static void UpdateFromRequest(this Habit habit, UpdateHabitRequest request)
    {
        habit.Name = request.Name;
        habit.Description = request.Description;
        habit.Type = request.Type;
        habit.EndDate = request.EndDate;

        habit.Frequency = new Frequency
        {
            Type = request.Frequency.Type,
            TimesPerPeriod = request.Frequency.TimesPerPeriod
        };

        habit.Target = new Target
        {
            Value = request.Target.Value,
            Unit = request.Target.Unit
        };

        if (request.Milestone != null)
        {
            habit.Milestone ??= new Milestone();
            habit.Milestone.Target = request.Milestone.Target;
        }

        habit.UpdatedAtUtc = DateTime.UtcNow;
    }
}
