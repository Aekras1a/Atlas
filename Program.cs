using System;

namespace Atlas
{
    internal class Program
    {
        static Settings Backup_Settings = new Settings();
        static BackupEngine Backup_Engine = new BackupEngine(Backup_Settings);
        
        static void Main(string[] args)
        {
            String Backup_File = Backup_Engine.CreateNewFileBackup();
            //TestDecryption(Backup_File);
        }

        static void TestDecryption(String pInputPath)
        {
            EncryptionEngine EE = new EncryptionEngine(Backup_Settings.Encryption_Password);
            EE.Decrypt(pInputPath);
        }
    }
}
