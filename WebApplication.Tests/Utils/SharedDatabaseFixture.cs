using System.Runtime.InteropServices;
using System.Data.Common;
using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

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
            string connectionString;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Using Sqlite for linux here
                connectionString = "Data Source=app.db";
                Connection = new SqliteConnection(connectionString);
            }
            else
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=sPortsTest;Trusted_Connection=True;MultipleActiveResultSets=true";
                Connection = new SqlConnection(connectionString);
            }

            //ContextOptions = new DbContextOptionsBuilder<SportsContext>().UseSqlServer(connectionString).Options;

            Seed();

            Connection.Open();
        }

        public SportsContext CreateContext(DbTransaction transaction = null)
        {
            SportsContext context;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Using Sqlite for linux here
                context = new SportsContext(new DbContextOptionsBuilder<SportsContext>().UseSqlite(Connection).Options);
            }
            else
            {
                context = new SportsContext(new DbContextOptionsBuilder<SportsContext>().UseSqlServer(Connection).Options);
            }

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
