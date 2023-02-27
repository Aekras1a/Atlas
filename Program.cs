using System;

namespace Atlas
{
    internal class Program
    {
        static Settings Backup_Settings = new Settings();
        static BackupEngine Backup_Engine = new BackupEngine(Backup_Settings);
        
        static void Main(string[] args)
        {
            Backup_Engine.CreateNewFileBackup();
        }

        static void TestDecryption(String pInputPath)
        {
            EncryptionEngine EE = new EncryptionEngine(Backup_Settings.Encryption_Password);
            EE.Decrypt(pInputPath);
        }
    }
}
