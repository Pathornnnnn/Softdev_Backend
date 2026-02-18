// ในไฟล์ Data/MyDbContext.cs
using Microsoft.EntityFrameworkCore;
using jangsom.Models;

namespace jangsom.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ตั้งค่าความสัมพันธ์สำหรับ Report -> User (Reporter)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany(u => u.ReportsAsReporter)
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict); // ป้องกันการลบ User แล้วข้อมูลพัง

            // ตั้งค่าความสัมพันธ์สำหรับ Report -> User (Technician)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Technician)
                .WithMany(u => u.ReportsAsTechnician)
                .HasForeignKey(r => r.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}