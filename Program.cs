using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

            string Backup_File = Backup_Engine.CreateNewFileBackupAsync().Result;
            TestDecryption(Backup_File);

            Timer.Stop();
            TimeSpan timeTaken = Timer.Elapsed;
            Debug.WriteLine("\n\n[*]Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
        }

        static void TestDecryption(String pInputPath)
        {
            Debug.WriteLine("[*] Decrypting Backup");
            EncryptionEngine EE = new EncryptionEngine(Backup_Settings.Encryption_Password);
            EE.Decrypt(pInputPath);
        }
    }
}
