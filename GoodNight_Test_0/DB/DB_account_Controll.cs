using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace GoodNight_Test_0
{
    class DB_account_Controll:DB_Controller
    {
        public async Task initializate_account(DB_account data)
        {
            SQLiteAsyncConnection db = GetConn();

            await db.DropTableAsync<DB_account>();
            await db.CreateTableAsync<DB_account>();

            await db.InsertAsync(data);

            var query = db.Table<DB_account>();
            List<DB_account> tamp = await query.ToListAsync();
        }
        public async void update_account(DB_account data)
        {
            SQLiteAsyncConnection db = GetConn();
            await db.UpdateAsync(data);
        }
        public async Task<DB_account> get_account()
        {
            SQLiteAsyncConnection db = GetConn();
            var query = db.Table<DB_account>();
            List<DB_account> tamp = await query.ToListAsync();
            return tamp[0];
        }
    }
}
