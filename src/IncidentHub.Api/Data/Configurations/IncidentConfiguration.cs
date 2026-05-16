using IncidentHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IncidentHub.Api.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(i => i.CreatedByUser)
            .WithMany(u => u.CreatedIncidents)
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.AssignedToUser)
            .WithMany(u => u.AssignedIncidents)
            .HasForeignKey(i => i.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Priority);
        builder.HasIndex(i => i.CreatedAtUtc);
        builder.HasIndex(i => i.AssignedToUserId);
    }
}


