using DesafioBackEndRDManipulacao.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class YouTubeDbContextFactory : IDesignTimeDbContextFactory<YouTubeDbContext>
{
    public YouTubeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<YouTubeDbContext>();
        optionsBuilder.UseSqlite("Data Source=YouTubeDatabase.db");

        return new YouTubeDbContext(optionsBuilder.Options);
    }
}
