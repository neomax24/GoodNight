using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Windows.Data;
using System.ComponentModel;
namespace GoodNight_Test_0
{
    [Table("DB_TimePointList")]
    class DB_TimePointList : INotifyPropertyChanged
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
        public TimeSpan TIME_POINT { get { return time_point; } set { time_point = value; NotifyPropertyChange("TIME_POINT"); NotifyPropertyChange("CanPickTime"); } }
        private bool isWork;
        public bool IS_WORK { get { return isWork; } set { isWork = value; } }
        public bool CanPickTime { get { return !isWork; } }

                public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件：
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
