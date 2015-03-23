using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;
using System.Collections.ObjectModel;

namespace GoodNight_Test_0.DB
{
    class DB_Friend_Controll:DB_Controller
    {
        public async void insertUpdate_Friend(DB_Friend data)
        {
            SQLiteAsyncConnection db = GetConn();
            if(await db.InsertAsync(data)!=0)
            {
                await db.UpdateAsync(data);
            }
        }
        public async Task<List<DB_Friend>> get_friendList()
        {
            SQLiteAsyncConnection db = GetConn();
            var query=db.Table<DB_Friend>();
            return await query.ToListAsync();
        }
        public async void delete_Friend(DB_Friend data)
        {
            SQLiteAsyncConnection db = GetConn();
            await db.DeleteAsync(data);
        }
    }
}
