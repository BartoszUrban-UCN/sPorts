using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WebApplication.Data;

namespace WebApplication.Tests.Utils
{
    public class SingleDatabaseSetup
    {
        protected DbContextOptions<SportsContext> ContextOptions { get; }
        protected SingleDatabaseSetup()
        {
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=sPortsTest;Trusted_Connection=True;MultipleActiveResultSets=true";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                connectionString = "Server=localhost;Database=sPortsTest;User Id=sa;Password=Password123;Trusted_Connection=False;MultipleActiveResultSets=true";
            }
            // Replace connectionString
            ContextOptions = new DbContextOptionsBuilder<SportsContext>().UseSqlServer(connectionString).Options;

            Seed();
        }

        private void Seed()
        {
            using (var context = new SportsContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                DbInitializer.SeedDb(context);
            }

        }
    }
}
