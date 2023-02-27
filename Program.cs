using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Atlas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Settings Backup_Settings = new Settings();
            BackupEngine Backup_Engine = new BackupEngine(Backup_Settings);

            //Backup_Engine.CreateNewFileBackup();
        }
    }
}
