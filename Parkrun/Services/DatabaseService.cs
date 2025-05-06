using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkrun.MVVM.Models;
using Parkrun.MVVM.Helpers;

namespace Parkrun.Services
{
    internal class DatabaseService
    {
        private readonly SQLiteAsyncConnection database;

        public DatabaseService()
        {
            database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath);
            database.CreateTableAsync<ParkrunData>().Wait();
        }

        public Task<List<ParkrunData>> GetDataAsync()
        {
            return database.Table<ParkrunData>().ToListAsync();
        }

        public Task<int> SaveDataAsync(ParkrunData data)
        {
            return database.InsertAsync(data);
        }

        public Task<int> DeleteDataAsync(ParkrunData data)
        {
             return database.DeleteAsync(data);
        }
    }
}
