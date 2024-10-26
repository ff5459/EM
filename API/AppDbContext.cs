using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
    }
}