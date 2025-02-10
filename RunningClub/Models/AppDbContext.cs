using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RunningClub.Models;

public class AppDbContext: IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<Race> Races { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<MemberClub> MemberClubs { get; set; }
    public DbSet<MemberRace> MemberRaces { get; set; }
    // public DbSet<Address> Addresses { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // builder.Entity<AppUser>().HasIndex(u => u.UserName).IsUnique();
        builder.Entity<MemberRace>().HasKey(mr => new { mr.MemberId, mr.RaceId });
        builder.Entity<MemberClub>().HasKey(mc => new { mc.MemberId, mc.ClubId });
        builder.Entity<Club>().OwnsOne(c => c.Address);
        builder.Entity<Race>().OwnsOne(r => r.Address);
        builder.Entity<MemberRace>().HasOne(mr=>mr.Member).WithMany(u=>u.Races).HasForeignKey(mr=>mr.MemberId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MemberRace>().HasOne(mr=>mr.Race).WithMany(r=>r.Members).HasForeignKey(mr=>mr.RaceId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MemberClub>().HasOne(mc=>mc.Member).WithMany(u=>u.Clubs).HasForeignKey(mr=>mr.MemberId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MemberClub>().HasOne(mc=>mc.Club).WithMany(c=>c.Members).HasForeignKey(mr=>mr.ClubId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AppUser>().HasMany(u=>u.AdminClubs).WithOne(c=>c.Admin).HasForeignKey(c=>c.AdminId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AppUser>().HasMany(u=>u.AdminRaces).WithOne(r=>r.Admin).HasForeignKey(r=>r.AdminId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Race>().HasOne(r=>r.Club).WithMany(c=>c.Races).HasForeignKey(r=>r.ClubId).OnDelete(DeleteBehavior.Cascade);
        // builder.Entity<AppUser>().HasMany(u => u.CompletedRaces).WithOne(c => c.Admin);
    }
}