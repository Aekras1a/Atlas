using System;
using System.Diagnostics;

namespace Atlas
{
    internal class Program
    {
        static Settings Backup_Settings = new Settings();
        static BackupEngine Backup_Engine = new BackupEngine(Backup_Settings);
        
        static void Main(string[] args)
        {
            Stopwatch Timer = new Stopwatch();
            Timer.Start();

            String Backup_File = Backup_Engine.CreateNewFileBackup();
            //TestDecryption(Backup_File);

            Timer.Stop();
            TimeSpan timeTaken = Timer.Elapsed;
            Debug.WriteLine("\n\n[*]Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
        }

        static void TestDecryption(String pInputPath)
        {
            EncryptionEngine EE = new EncryptionEngine(Backup_Settings.Encryption_Password);
            EE.Decrypt(pInputPath);
        }
    }
}
