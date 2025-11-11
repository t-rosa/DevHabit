using DevHabit.Api.Modules.Habits;
using DevHabit.Api.Modules.HabitTags;
using DevHabit.Api.Modules.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Modules.HabitTags;

public sealed class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(ht => new { ht.HabitId, ht.TagId });

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ht => ht.TagId);

        builder.HasOne<Habit>()
            .WithMany(h => h.HabitTags)
            .HasForeignKey(ht => ht.HabitId);
    }
}
