using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;
using System.Collections.ObjectModel;

namespace GoodNight_Test_0
{
    class DB_Controller
    {
        private List<DB_TimePeriodList> list_timePeriodList;
        public  List<DB_TimePeriodList> get_timePeriodList { get { return list_timePeriodList; } }
        private List<DB_TimePointList> list_timePointList;
        public List<DB_TimePointList> get_timePointList { get { return list_timePointList; } }
        public async Task CreatTable()
        {
            if (await isDataBaseExist() == false)
            {
                SQLiteAsyncConnection conn = GetConn();
                await conn.CreateTableAsync<DB_TimePeriodList>();
                await conn.CreateTableAsync<DB_TimePointList>();
                insert_TimePeriodList(new DB_TimePeriodList("游戏", 5, false));
                insert_TimePeriodList(new DB_TimePeriodList("微博", 15, false));
                insert_TimePointList(new DB_TimePointList("睡觉", new TimeSpan(20, 30, 30), false));
                insert_TimePointList(new DB_TimePointList("学习", new TimeSpan(20, 30, 30), false));
            }
            await _get_timePeriodList();
            await _get_timePointList();
        }
        private async Task _get_timePeriodList()
        {
            list_timePeriodList = await getTable_TimePeriodList();
        }
        private async Task _get_timePointList()
        {
            list_timePointList = await getTable_TimePointList();
        }
        private SQLiteAsyncConnection GetConn()
        {
            return new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\GoodNight.db");
        }
        public async void insert_TimePeriodList(DB_TimePeriodList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.InsertAsync(data);
        }
        public async void insert_TimePointList(DB_TimePointList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.InsertAsync(data);
        }
        public async void delete_TimePeriodList(DB_TimePeriodList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.DeleteAsync(data);
        }
        public async void delete_TimePointList(DB_TimePointList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.DeleteAsync(data);
        }
        public async Task update_TimePeriodList(DB_TimePeriodList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.UpdateAsync(data);
        }
        public async Task update_TimePeriodListAll()
        {
            SQLiteAsyncConnection conn=GetConn();
            await conn.UpdateAllAsync(list_timePeriodList);
        }

        private static async System.Threading.Tasks.Task<bool> isDataBaseExist()
        {
            string filePath = "GoodNight.db";
            bool isFileExist = true;
            try
            {
                Windows.Storage.StorageFile sf = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
            }
            catch
            {
                isFileExist = false;
            }
            return isFileExist;
        }
        public async Task<List<DB_TimePeriodList>> getTable_TimePeriodList()
        {
            SQLiteAsyncConnection conn = GetConn();
            var query = conn.Table<DB_TimePeriodList>();
            List<DB_TimePeriodList> result = await query.ToListAsync();
            return result;
        }
        public async Task<List<DB_TimePointList>> getTable_TimePointList()
        {
            SQLiteAsyncConnection conn = GetConn();
            var query = conn.Table<DB_TimePointList>();
            List<DB_TimePointList> result = await query.ToListAsync();
            return result;
        }
        public async Task reflesh_timePeriod()
        {
            await _get_timePeriodList();
        }
        public async Task reflesh_timePoint()
        {
            await _get_timePointList();
        }
    }
}
