using Microsoft.EntityFrameworkCore;
using website_performance.Models;

namespace website_performance.Data
{
    // View > Other Windows > Package Manager Console
    // Add-Migration InitialCreate
    // Update-Database
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PerformanceDbModel> Performances { get; set; }
    }
}
