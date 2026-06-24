using League_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace League_Backend.Contexts
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options): base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
