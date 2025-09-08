using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Data
{
    public class ApplicationDbContext : DbContext
    {
        public  DbSet<ShortUrl> ShortUrls { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrl>().HasKey(s => s.ShortCode);
        }
    }
}
