using System.Runtime.InteropServices;
using System.Data.Common;
using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using WebApplication.Data;

namespace WebApplication.Tests.Utils
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public DbConnection Connection { get; }

        public SharedDatabaseFixture()
        {
            // Local DB sPortsTest instead of normal
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=sPortsTest;Trusted_Connection=True;MultipleActiveResultSets=true";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                connectionString = "Server=localhost;Database=sPortsTest;User Id=sa;Password=Password123;Trusted_Connection=False;MultipleActiveResultSets=true";
            }
            //ContextOptions = new DbContextOptionsBuilder<SportsContext>().UseSqlServer(connectionString).Options;

            Connection = new SqlConnection(connectionString);

            Connection.Open();

            Seed();
        }

        public SportsContext CreateContext(DbTransaction transaction = null)
        {
            var context = new SportsContext(new DbContextOptionsBuilder<SportsContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        DbInitializer.InitializeDb(context);
                    }
                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
