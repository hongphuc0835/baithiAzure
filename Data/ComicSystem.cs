using Microsoft.EntityFrameworkCore;
using baithi.Models;
namespace baithi.Data
{
    public class ComicSystem : DbContext
    {
        public ComicSystem(DbContextOptions<ComicSystem> options) : base(options)
        { }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Rentals> Rentals { get; set; }
        public DbSet<RentalDetails> RentalDetails { get; set; }
        public DbSet<ComicBooks> ComicBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>()
            .HasMany(c => c.Rentals)
            .WithOne(r => r.Customers)
            .HasForeignKey(r => r.CustomerID)
            ;
            modelBuilder.Entity<Rentals>()
            .HasMany(r => r.RentalDetails)
            .WithOne(rd => rd.Rentals)
            .HasForeignKey(rd => rd.RentalID)
            ;
            modelBuilder.Entity<ComicBooks>()
            .HasMany(c => c.RentalDetails)
            .WithOne(rd => rd.ComicBooks)
            .HasForeignKey(rd => rd.ComicBookID);
        }
    }
}

