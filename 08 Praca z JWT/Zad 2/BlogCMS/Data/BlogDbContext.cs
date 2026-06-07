using BlogCMS.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<UserAccount> Users => Set<UserAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>()
                .HasIndex(user => user.Username)
                .IsUnique();

            modelBuilder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = 1,
                Username = "Grzegorz",
                Password = "TajneHaslo_1234",
                Role = "Admin"
            });
        }
    }
}
