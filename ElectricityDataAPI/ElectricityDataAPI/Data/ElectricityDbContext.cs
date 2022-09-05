using ElectricityDataAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricityDataAPI.Data
{
    public class ElectricityDbContext : DbContext
    {
        public ElectricityDbContext(DbContextOptions<ElectricityDbContext> options) : base(options)
        {
        }

        public DbSet<ElectricityReport> ElectricityReports { get; set; }
        public DbSet<RealEstate> RealEstates { get; set; }
    }
}
