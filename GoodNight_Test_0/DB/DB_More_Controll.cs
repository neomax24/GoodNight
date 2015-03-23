using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace GoodNight_Test_0
{
    class DB_More_Controll:DB_Controller
    {
        public async void insertUpdate_More(DB_More data)
        {
            SQLiteAsyncConnection db = GetConn();
            if (await db.InsertAsync(data) != 0)
            {
                await db.UpdateAsync(data);
            }
        }
        public async Task<DB_More> get_more()
        {
            SQLiteAsyncConnection db = GetConn();
            var query = db.Table<DB_More>();
            List<DB_More> tamp = await query.ToListAsync();
            return tamp[0];
        }
    }
}
