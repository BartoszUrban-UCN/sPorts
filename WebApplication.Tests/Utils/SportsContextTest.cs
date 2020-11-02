using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Tests.Utils
{
    public class SportsContextTest
    {
        protected DbContextOptions<SportsContext> ContextOptions { get; }
        protected SportsContextTest()
        {
            // Replace username and password
            // var linuxconnectionString = "Server=localhost;Database=sPortsTest;User Id=sa;Password=Password123;Trusted_Connection=False;MultipleActiveResultSets=true";

            // Local DB sPortsTest instead of normal
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=sPortsTest;Trusted_Connection=True;MultipleActiveResultSets=true";
            
            // Replace connectionString
            ContextOptions = new DbContextOptionsBuilder<SportsContext>().UseSqlServer(connectionString).Options;

            Seed();
        }

        private void Seed()
        {
            using (var context = new SportsContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                DbInitializer.InitializeDB(context);
            }

        }
    }
}
