using KinoStars.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoStars.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Review> Reviews { get; set; }
    }
}
