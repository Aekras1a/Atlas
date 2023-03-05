using Atlas.Atlas;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Atlas
{
    internal class Program
    {
        private static Settings backupSettings = new Settings();

        private static async Task Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            CommunicationManager communicationManager = new CommunicationManager(backupSettings);
            await communicationManager.DoPostRequestAsync(backupSettings.serverEndpoints.getCommand);

            timer.Stop();
            String timeTaken = timer.Elapsed.ToString(@"m\:ss\.fff");
            Debug.WriteLine($"[*] (MAIN) Time taken: {timeTaken}");
        }

        private static async Task doBackup()
        {
            BackupEngine backupEngine = new BackupEngine(backupSettings);

            string backupFile = await backupEngine.CreateNewFileBackupAsync();
            TestDecryption(backupFile, backupSettings);
        }

        private static void TestDecryption(string backupFilePath, Settings backupSettings)
        {
            EncryptionEngine encryptionEngine = new EncryptionEngine(backupSettings.encryptionPassword);
            encryptionEngine.Decrypt(backupFilePath);
        }
    }
}