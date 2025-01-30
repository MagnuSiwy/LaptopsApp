using Microsoft.EntityFrameworkCore;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class LaptopContext : DbContext
    {
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Producer> Producers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dataBasePath = @"C:\Users\student\Documents\LaptopsApp\laptops.db";
            optionsBuilder.UseSqlite($"Data Source={dataBasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Laptop>()
                .HasOne(l => l.Producer)
                .WithMany()
                .HasForeignKey(l => l.ProducerId);

            base.OnModelCreating(modelBuilder);
        }
    }
}