using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonNotesApp.Data
{
    public class DatabaseContext : IAsyncDisposable
    {
        private const string DbName = "TodoApp.db";
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbName);

        private SQLiteAsyncConnection _connection;

        private SQLiteAsyncConnection Database => (_connection ??= new SQLiteAsyncConnection(DbPath,SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
        private async Task CreateTableIfNotExists<TTable>() where TTable : class, new()
        {
            await Database.CreateTableAsync<TTable>();

        }
        private async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class , new()
        {
            await CreateTableIfNotExists<TTable>();
            return Database.Table<TTable>();
        }
        public async Task<TTable> GetItemByKeyAsync<TTable>(object primaryKey) where TTable : class ,new()
        {
            var table = await GetTableAsync<TTable>();
            return await Database.GetAsync<TTable>(primaryKey);
        }
        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class ,new()
        {
            var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }
        public async Task<IList<TTable>> GetFilteredAsync<TTable>(Expression<Func<TTable,bool>> predicate) where TTable : class ,new()
        {
            var table = await GetTableAsync<TTable>();
            return await table.Where(predicate).ToListAsync();
        }

        public async Task<TTable> AddItemAsync<TTable>(TTable item) where TTable : class , new()
        {
            await CreateTableIfNotExists<TTable>();
            await Database.InsertAsync(item);
            return item;
        }
        public async Task<bool> UpdateItemAsync<TTable>(TTable item) where TTable : class , new()
        {
            await CreateTableIfNotExists<TTable>();
            return await Database.UpdateAsync(item) > 0;
        }
        public async Task<bool> DeleteItemAsync<TTable>(TTable item) where TTable : class , new()
        {
            await CreateTableIfNotExists<TTable>();
            return await Database.DeleteAsync(item) > 0;
        }

        public async ValueTask DisposeAsync() => await _connection?.CloseAsync();

        
    }
}
