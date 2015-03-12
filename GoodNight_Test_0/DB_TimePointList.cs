using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace GoodNight_Test_0
{
    [Table("DB_TimePointList")]
    class DB_TimePointList
    {
        public DB_TimePointList()
        { }
        public DB_TimePointList(string _name, TimeSpan _time, bool _isWork)
        {
            NAME = _name;
            TIME_POINT = _time;
            IS_WORK = _isWork;
        }
        private int id;
        [PrimaryKey,AutoIncrement]
        public int ID { get { return id; } set { id = value; } }
        private string name;
        public string NAME { get { return name; } set { name = value; } }
        private TimeSpan time_point;
        public TimeSpan TIME_POINT { get { return time_point; } set { time_point = value; } }
        private bool isWork;
        public bool IS_WORK { get { return isWork; } set { isWork = value; } }
    }
}
