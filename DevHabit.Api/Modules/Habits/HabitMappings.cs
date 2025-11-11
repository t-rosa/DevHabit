using DevHabit.Api.Modules.Habits.DTOs;

namespace DevHabit.Api.Modules.Habits;

internal static class HabitMappings
{
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
