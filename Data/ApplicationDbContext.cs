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

        public DbSet<WebsiteDbModel> Websites { get; set; }
        public DbSet<SitemapDbModel> Sitemaps { get; set; }
    }
}
