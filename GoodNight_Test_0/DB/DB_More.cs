﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace GoodNight_Test_0
{
    [Table("DB_More")]
    class DB_More
    {
        public DB_More()
        { }

        public DB_More(string __nickNmae,string __declaration,int __sex,string __avatarPath)
        {
            _nickName = __nickNmae;
            _declaration = __declaration;
            _sex = __sex;
            _avatarPath = __avatarPath;
        }
        private int id;
        [PrimaryKey,AutoIncrement]
        public int ID { get { return id; } set { id = value; } }
        private string _nickName;
        public string nickName { get { return _nickName; } set { _nickName = value; } }
        private string _declaration;
        public string declaration { get { return _declaration; } set { _declaration = value; } }
        private string _avatarPath;
        public string avatarPath { get { return _avatarPath; } set { _avatarPath = value; } }
        private int _sex;
        public int sex { get { return _sex; } set { _sex = value; } }

    }
}
