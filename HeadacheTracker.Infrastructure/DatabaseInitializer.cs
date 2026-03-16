using HeadacheTracker.Domain.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadacheTracker.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseInitializer(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task InitializeAsync()
        {
            await _database.CreateTableAsync<HeadacheEntry>();
            await _database.CreateTableAsync<MedicationEntry>();
        }
    }
}
