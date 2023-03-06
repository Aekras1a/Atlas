using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas
{
    internal class CommunicationManager
    {
        private static ConnectionSettings connectionSettings = new ConnectionSettings();
        private static BackupSettings backupSettings = new BackupSettings();

        private static ConnectionManager connectionManager = new ConnectionManager(connectionSettings);
        private static BackupEngine backupEngine = new BackupEngine(backupSettings);

        private static Machine machine = new Machine();

        public async Task getCommand()
        {
            int responce = await connectionManager.SendPostRequestAsync(connectionSettings.serverEndpoints.getCommand, new Dictionary<String, String>() { { "machine_id", machine.hardwareId } });
            switch (responce)
            {
                case (int)ConnectionSettings.vaildResponces.NA:
                    {
                        break;
                    }
                case (int)ConnectionSettings.vaildResponces.doFileBackup:
                    {
                        string backupFile = await backupEngine.CreateNewFileBackupAsync();
                        TestDecryption(backupFile, backupSettings);
                        break;
                    }
            }
        }

        private static void TestDecryption(string backupFilePath, BackupSettings backupSettings)
        {
            EncryptionEngine encryptionEngine = new EncryptionEngine(backupSettings.encryptionPassword);
            encryptionEngine.Decrypt(backupFilePath);
        }
    }
}