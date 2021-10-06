using Microsoft.EntityFrameworkCore;

namespace Back_End.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options): base(options)
        {
        }

        public DbSet<MovieDescription> MovieDescriptions { get; set; }
    }
}