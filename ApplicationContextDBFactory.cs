using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MovieRatingAPI.Data;

namespace MovieRatingAPI

{
    public class ApplicationContextDBFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            optionsBuilder.UseSqlServer(
                "Server=.;Database=Movies;Trusted_Connection=True;TrustServerCertificate=True;"
            );

            return new DataContext(optionsBuilder.Options);
        }
    }
}