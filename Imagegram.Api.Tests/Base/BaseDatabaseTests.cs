using System;
using Imagegram.Api.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imagegram.Api.Tests
{
    public abstract class BaseDatabaseTests : IDisposable
    {        
        protected SqliteConnection Connection { get; private set; }

        public static readonly LoggerFactory _consoleLogger =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });
        private readonly DbContextOptions<ApplicationContext> _options;
        private ApplicationContext _database;

        public BaseDatabaseTests()
        {
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseLoggerFactory(_consoleLogger)
                .UseSqlite(Connection)
                .Options;

            SaveToDatabase();
        }

        protected void SaveToDatabase()
        {
            _database?.SaveChanges();
            ConnectToDatabase();
        }

        protected ApplicationContext ConnectToDatabase()
        {
            _database = new ApplicationContext(_options);
            _database.Database.EnsureCreated();
            return _database;
        }

        protected ApplicationContext GetDatabase()
        {           
            return _database;
        }

        public virtual void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}