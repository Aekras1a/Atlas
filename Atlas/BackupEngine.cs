using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using ICSharpCode.SharpZipLib.Zip;

namespace Atlas
{
    /// <summary>
    /// Class for managing backups
    /// </summary>
    internal class BackupEngine
    {
        private readonly Settings backupSettings = null;
        private readonly EncryptionEngine encryptionEngine = null;

        /// <summary>
        /// Initializes a new instance of the BackupEngine class.
        /// </summary>
        /// <param name="pBackup_Settings">The settings for the backup.</param>
        public BackupEngine(Settings pBackup_Settings)
        {
            backupSettings = pBackup_Settings;
            encryptionEngine = new EncryptionEngine(backupSettings.encryptionPassword);
        }

        private static string GetRelativePath(string basePath, string targetPath)
        {
            var baseUri = new Uri(basePath.EndsWith("/") ? basePath : basePath + "/");
            var targetUri = new Uri(targetPath);

            var relativeUri = baseUri.MakeRelativeUri(targetUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        private async Task<string> PackageBackupAsync(DirectoryInfo backupDir)
        {
            Debug.WriteLine("\n\n[*] Packaging Backup");

            string zipPath = Path.Combine(backupSettings.rootBackupDir, DateTime.Now.ToLongDateString());

            using (var fileStream = new FileStream(zipPath, FileMode.Create))
            {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    foreach (var dirPath in backupSettings.dirsToBackup)
                    {
                        var dirInfo = new DirectoryInfo(dirPath);
                        var entryName = dirInfo.Name + ".zip";

                        var entry = archive.CreateEntry(entryName);

                        using (var entryStream = entry.Open())
                        {
                            await CopyFilesAsync(dirPath, entryStream);
                        }
                    }
                }
            }

            if (backupSettings.doEncrypt)
            {
                Debug.WriteLine("[*] Encrypting Backup");
                encryptionEngine.Encrypt(zipPath);
                zipPath = Path.ChangeExtension(zipPath, "backup");
            }
            else
            {
                File.Move(zipPath, Path.ChangeExtension(zipPath, "zip"));
            }

            Directory.Delete(backupDir.FullName, true);

            return zipPath;
        }

        private async Task CopyFilesAsync(string sourceDir, Stream targetStream)
        {
            var totalFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories).Length;
            var filesProcessed = 0;

            using (var zipStream = new ZipOutputStream(targetStream))
            {
                zipStream.SetLevel((int)CompressionLevel.Optimal);

                var buffer = new byte[4096];

                foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var entryName = GetRelativePath(sourceDir, file);

                    var entry = new ZipEntry(entryName);
                    zipStream.PutNextEntry(entry);

                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        int bytesRead;

                        do
                        {
                            bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                            await zipStream.WriteAsync(buffer, 0, bytesRead);
                        } while (bytesRead > 0);
                    }

                    zipStream.CloseEntry();

                    filesProcessed++;
                    var percentageComplete = Math.Round((double)filesProcessed / totalFiles * 100, 2);
                    Debug.WriteLine($"[*] {sourceDir} : {percentageComplete}%");
                }
            }
        }

        /// <summary>
        /// Creates a new file backup.
        /// </summary>
        /// <returns>The path to the backup file.</returns>
        public async Task<string> CreateNewFileBackupAsync()
        {
            var backupDir = Directory.CreateDirectory(Path.Combine(backupSettings.rootBackupDir, "Temp"));

            List<Task> backupTasks = new List<Task>();
            foreach (string dirPath in backupSettings.dirsToBackup)
            {
                Debug.WriteLine($"[*] Backing Up: {dirPath}");

                string dirCopyDir = Path.Combine(backupDir.FullName, new DirectoryInfo(dirPath).Name);
                backupTasks.Add(Task.Run(async () =>
                {
                    using (var stream = new FileStream(Path.Combine(backupDir.FullName, dirCopyDir + "_backup.zip"), FileMode.Create))
                    {
                        await CopyFilesAsync(dirPath, stream);
                    }
                }));
            }

            await Task.WhenAll(backupTasks);

            return await PackageBackupAsync(backupDir);
        }
    }
}