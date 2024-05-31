using Microsoft.EntityFrameworkCore;

namespace votesystem_csharp.Models;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Election> Elections { get; set; } = null!;
    public DbSet<Candidate> Candidates { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vote>().HasKey(u => new { u.UserId, u.ElectionId });
        modelBuilder.Entity<User>().HasAlternateKey(u => new { u.Login });
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Election)
            .WithMany(e => e.Candidates)
            .HasForeignKey(c => c.ElectionId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.User)
            .WithMany(u => u.Votes)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Election)
            .WithMany(e => e.Votes)
            .HasForeignKey(v => v.ElectionId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Candidate)
            .WithMany(c => c.Votes)
            .HasForeignKey(v => v.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}