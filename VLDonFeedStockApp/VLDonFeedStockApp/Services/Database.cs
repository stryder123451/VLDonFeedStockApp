using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;

namespace VLDonFeedStockApp.Services
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;
        public Database(string _dbPath)
        {
            _database = new SQLiteAsyncConnection(_dbPath);
            _database.CreateTableAsync<Workers>();
        }


        public Task<List<Workers>> GetUsersAsync()
        {
            return _database.Table<Workers>().ToListAsync();
        }

        public Task<Workers> GetUserAsync(string login)
        {
            try
            {
                if (login != null)
                {
                    return _database.Table<Workers>().FirstOrDefaultAsync(x => x.Login == login);
                }
                else
                {
                    return _database.Table<Workers>().FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task Login(Workers _user)
        {

            if (_database.Table<Workers>().CountAsync().Result == 0)
            {
                await SaveUserAsync(_user);
            }
            else
            {
                await UpdateUserAsync(_user);
            }

        }

        public Task<int> UpdateUserAsync(Workers user)
        {
            var _user = _database.Table<Workers>().FirstOrDefaultAsync().Result;
            var _localId = _user.LocalId;
            _user = user;
            _user.LocalId = _localId;
            return _database.UpdateAsync(_user);
        }

        public Task<int> SaveUserAsync(Workers user)
        {
            return _database.InsertAsync(user);
        }

    }
}
