using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Models;

namespace AttendanceDatabase.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<EventAttendanceRecord> EventAttendanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure EventAttendanceRecord entity
            modelBuilder.Entity<EventAttendanceRecord>(entity =>
            {
                // EventName is required and has a maximum length of 100 characters
                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Date is required
                entity.Property(e => e.Date)
                    .IsRequired();

                // Tags is optional but has a max length of 200 characters
                entity.Property(e => e.Tags)
                    .HasMaxLength(200);

                // AttendanceCount defaults to 0
                entity.Property(e => e.AttendanceCount)
                    .HasDefaultValue(0);

                // IsFlagged defaults to false
                entity.Property(e => e.IsFlagged)
                    .HasDefaultValue(false);

                // Optional: Configure indexes for faster lookups (e.g., by EventName and Date)
                entity.HasIndex(e => e.EventName);
                entity.HasIndex(e => e.Date);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25);

            });
        }
    }
}
