using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Tests.Utils
{
    public class SportsContextTest
    {
        protected DbContextOptions<SportsContext> ContextOptions { get; }

        protected SportsContextTest()
        {
            //Local DB sPortsTest instead of normal
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=sPortsTest;Trusted_Connection=True;MultipleActiveResultSets=true";
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
