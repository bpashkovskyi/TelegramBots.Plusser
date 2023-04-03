using Microsoft.EntityFrameworkCore;

using TelegramBots.BotPlusser.Domain.Entities;

namespace TelegramBots.BotPlusser.Infrastructure;

public class PlusserContext : DbContext
{
    public PlusserContext(DbContextOptions<PlusserContext> options)
        : base(options)
    {
    }

    public DbSet<Gathering> Gatherings { get; set; } = null!;

    public DbSet<Group> Groups { get; set; } = null!;

    public DbSet<Member> Members { get; set; } = null!;

    public DbSet<Attendee> Attendees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("plusser");

        modelBuilder.Entity<Gathering>()
            .HasMany(gathering => gathering.Attendees)
            .WithOne(attendee => attendee.Gathering)
            .HasForeignKey(attendee => attendee.GatheringId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Gathering>()
            .HasOne(gathering => gathering.Creator)
            .WithMany()
            .HasForeignKey(gathering => gathering.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Member>()
            .HasMany(member => member.Attendees)
            .WithOne(attendee => attendee.Member)
            .HasForeignKey(attendee => attendee.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Member>()
            .HasIndex(member => member.TelegramId)
            .IsUnique();

        modelBuilder.Entity<Group>()
            .HasMany(group => group.Gatherings)
            .WithOne(gathering => gathering.Group)
            .HasForeignKey(gathering => gathering.GroupId);

        modelBuilder.Entity<Group>()
            .Ignore(group => group.NonDraftGatherings);

        modelBuilder.Entity<Group>()
            .HasIndex(group => group.TelegramId)
            .IsUnique();
    }
}