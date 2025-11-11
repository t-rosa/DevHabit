using DevHabit.Api.Modules.Habits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Modules.Tags;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasMaxLength(500);

        builder.Property(h => h.Name).IsRequired().HasMaxLength(50);

        builder.Property(h => h.Description).HasMaxLength(500);

        builder.HasIndex(t => new { t.Name }).IsUnique();
    }
}
