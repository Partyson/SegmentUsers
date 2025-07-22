using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SegmentUsers.Domain.Entities;

namespace SegmentUsers.Infrastructure.ModelsConfiguration;

public class SegmentConfiguration : IEntityTypeConfiguration<Segment>
{
    public void Configure(EntityTypeBuilder<Segment> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasMany(s => s.Users)
            .WithMany(u => u.Segments);
    }
}