using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Models;

namespace Thunders.TechTest.Infrastructure.Data
{
    public class TollDataContext(DbContextOptions<TollDataContext> options) : DbContext(options)
    {
        public DbSet<TollUsage> TollUsages { get; set; }
        public DbSet<HourlyCityReport> HourlyCityReports { get; set; }
        public DbSet<TopTollPlazasReport> TopTollPlazasReports { get; set; }
        public DbSet<TollUsageCountReport> TollUsageCountReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TollUsage>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TollUsage>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TollUsage>()
                .Property(t => t.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HourlyCityReport>().HasNoKey();

            modelBuilder.Entity<HourlyCityReport>()
                .Property(t => t.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TopTollPlazasReport>().HasNoKey();

            modelBuilder.Entity<TopTollPlazasReport>()
                .Property(t => t.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TollUsageCountReport>().HasNoKey();

        }
    }
}
