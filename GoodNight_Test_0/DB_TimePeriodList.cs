using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace GoodNight_Test_0
{
    [Table("DB_TimePeriodList")]
    class DB_TimePeriodList
    {
        public DB_TimePeriodList()
        { }
        public DB_TimePeriodList(string _name,int _time_period,bool _isOpen)
        {
            name = _name;
            time_period = _time_period;
            isWork = _isOpen;
        }
        private int id;
        [PrimaryKey,AutoIncrement]

        public int ID { get { return id; } set { id = value; } }

        private string name;
        public string NAME { get { return name; } set { name = value; } }

        private int time_period;
        public int TIME_PERIOD { get { return time_period; } set { time_period = value; } }
        public int TIME_PERIOD_second { get { return time_period * 60; } }
        private bool isWork;
        public bool IS_WORK { get { return isWork; } set { isWork = value; } }

        private string timeStart;
        public string TIMESTART { get { return timeStart; } set { timeStart = value; } }
        private string timeEnd;
        public string TIMEEND { get { return timeEnd; } set { timeEnd = value; } }


        private int timePeriod_barValue;
        public int TimePeriod_barValue { get { return timePeriod_barValue; } set { timePeriod_barValue = value; } }
        public int get_timePeriod_barValue 
        { 
            get 
            {
                if (IS_WORK == true)
                    TimePeriod_barValue = (int)(DateTime.Now.Subtract(DateTime.Parse(timeStart)).TotalSeconds);
                return TimePeriod_barValue;
            }
        }

    }

}
