using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Images> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Document - User (many-to-one)
            modelBuilder.Entity<Document>()
                .HasOne(d => d.CreatedByUser)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // User - Role (many-to-one)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Image (one-to-one)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Image)
                .WithOne(i => i.User)
                .HasForeignKey<User>(u => u.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Role - Permissions (one-to-one)
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Permission)
                .WithOne(p => p.Role)
                .HasForeignKey<Role>(r => r.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ตั้งชื่อ table ชัดเจน (optional)
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Permissions>().ToTable("Permissions");
            modelBuilder.Entity<Images>().ToTable("Images");
            modelBuilder.Entity<Document>().ToTable("Documents");
        }
    }
}
