using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Windows.Data;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace GoodNight_Test_0
{
    [Table("DB_Friend")]
    class DB_Friend
    {
        private int id;
        [PrimaryKey, AutoIncrement]
        public int ID { get { return id; } set { id = value; } }
        private string name;
        public string NAME { get { return name; } set { name = value; } }
        private string _userID;
        public string userID { get { return _userID; } set { _userID = value; } }
        private string image_path;
        public string IMAGE_PATH { get { return image_path; } set { image_path = value; } }
    }
}
