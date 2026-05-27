using MovieRatingAPI.Data;
using Microsoft.EntityFrameworkCore;
using MovieRatingAPI.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MovieRatingAPI.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Movies> Movies { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        // public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
modelBuilder.Entity<Movies>()
    .HasMany(m => m.Reviews)
    .WithOne(r => r.Movie)
    .HasForeignKey(r => r.MovieId);

       
        }
    }
}