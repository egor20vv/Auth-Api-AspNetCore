using AuthApi.Data.Models.User;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Data;

public class MainDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { Database.EnsureCreated(); }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
    }

}
