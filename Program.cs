using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Atlas
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Settings backupSettings = new Settings();
            BackupEngine backupEngine = new BackupEngine(backupSettings);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            string backupFile = await backupEngine.CreateNewFileBackupAsync();
            TestDecryption(backupFile, backupSettings);

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            Debug.WriteLine($"[*] Time taken: {timeTaken}", timeTaken.ToString(@"m\:ss\.fff"));
        }

        private static void TestDecryption(string backupFilePath, Settings backupSettings)
        {
            Debug.WriteLine("[*] Decrypting Backup");
            EncryptionEngine encryptionEngine = new EncryptionEngine(backupSettings.encryptionPassword);
            encryptionEngine.Decrypt(backupFilePath);
        }
    }
}