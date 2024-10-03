using Microsoft.EntityFrameworkCore;

namespace First_MVC_Project.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get;set; }   
        public DbSet<Goal> Tasks { get;set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goal>()
                .Property(t => t.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
