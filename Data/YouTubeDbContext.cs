using DesafioBackEndRDManipulacao.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackEndRDManipulacao.Data
{
    public class YouTubeDbContext : DbContext
    {
        public YouTubeDbContext(DbContextOptions<YouTubeDbContext> options) : base(options)
        {
        }

        public DbSet<Video> Videos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=YouTubeVideos.db"); 
            }
        }
    }
}
