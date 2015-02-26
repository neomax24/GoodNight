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
            isOpen = _isOpen;
        }
        private int id;
        [PrimaryKey,AutoIncrement]

        public int ID { get { return id; } set { id = value; } }

        private string name;
        public string NAME { get { return name; } set { name = value; } }

        private int time_period;
        public int TIME_PERIOD { get { return time_period; } set { time_period = value; } }

        private bool isOpen;
        public bool IS_POEN { get { return isOpen; } set { isOpen = value; } }
    }

}
