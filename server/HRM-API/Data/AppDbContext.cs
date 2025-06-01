using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Images> Images { get; set; }

    }
}
