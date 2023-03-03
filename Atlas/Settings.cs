using System;
using System.Collections.Generic;

namespace Atlas
{
    internal class Settings
    {
        public string Root_Backup_Dir = "E:\\_BACKUPS";
        public List<string> Backup_Dirs = new List<string>() {
            //"E:\\Desktop",
            //"E:\\Documents\\_My Stuff",
            //"E:\\Downloads",
            //"E:\\Music",
            //"E:\\Videos",
            "E:\\Pictures"
        };

        public bool Encrypt = true;
        public String Encryption_Password = "password";

        private enum Threads
        {
            Auto = -1,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }

        public int Max_Threads = (int)Threads.Two;
    }
}
