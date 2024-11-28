using MetroAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MetroAPI.Data
{
    public class AppDbContext : DbContext
    {
      //  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Station> Stations { get; set; }
    }
}
