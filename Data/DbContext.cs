using Microsoft.EntityFrameworkCore;
using Practice_Application.Models;

namespace Practice_Application.Data
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
