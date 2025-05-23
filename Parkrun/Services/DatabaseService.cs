﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkrun.MVVM.Models;
using Parkrun.MVVM.Helpers;

namespace Parkrun.Services
{
    internal static class DatabaseService
    {
        public readonly static SQLiteAsyncConnection database;


        static DatabaseService()
        {
            database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath);
            database.CreateTableAsync<ParkrunData>().Wait();
        }
        

        public static Task<List<ParkrunData>> GetDataAsync()
        {
            return database.Table<ParkrunData>().ToListAsync();
        }

        public static List<ParkrunData> GetDataSync()
        {
            return database.Table<ParkrunData>().ToListAsync().GetAwaiter().GetResult();
        }


        public static Task<int> SaveDataAsync(ParkrunData data)
        {
            return database.InsertAsync(data);
        }

        public static Task<int> DeleteDataAsync(ParkrunData data)
        {
             return database.DeleteAsync(data);
        }

        public static Task<int> DeleteAllDataAsync()
        {
            return database.DeleteAllAsync<ParkrunData>();
        }
    }
}
