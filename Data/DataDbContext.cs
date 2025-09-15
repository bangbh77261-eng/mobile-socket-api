using Microsoft.EntityFrameworkCore;
using mobile_api_test.Models;

namespace mobile_api_test.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MessageUser> MessageUser { get; set; }
    }
}
