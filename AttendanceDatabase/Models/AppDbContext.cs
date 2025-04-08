using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<CategoryTags> CategoryTags { get; set; }
}