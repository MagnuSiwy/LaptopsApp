using Microsoft.EntityFrameworkCore;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class LaptopContext : DbContext
    {
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Producer> Producers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=laptops.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Laptop>()
                .HasOne(l => l.Producer)
                .WithMany()
                .HasForeignKey(l => l.ProducerId); // Use foreign key property

            base.OnModelCreating(modelBuilder);
        }
    }
}