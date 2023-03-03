using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas
{
    internal class BackupEngine
    {
        private readonly Settings Backup_Settings = null;
        private readonly EncryptionEngine EncryptionEngine = null;

        public BackupEngine(Settings pBackup_Settings)
        {
            Backup_Settings = pBackup_Settings;
            EncryptionEngine = new EncryptionEngine(Backup_Settings.Encryption_Password);
        }

        private void CopyFiles(string Source_Path, string Target_Path)
        {
            int Max_Threads = Backup_Settings.Max_Threads;

            if (!Directory.Exists(Source_Path))
            {
                Debug.WriteLine("[*] Source directory does not exist: " + Source_Path);
                return;
            }

            if (!Directory.Exists(Target_Path))
            {
                Debug.WriteLine("[*] Target directory does not exist: " + Target_Path);
                return;
            }

            try
            {
                string sourceRoot = Path.GetFullPath(Source_Path);
                string targetRoot = Path.GetFullPath(Target_Path);

                // Get a list of all the directories to create
                var directories = Directory.GetDirectories(Source_Path, "*", SearchOption.AllDirectories)
                    .Select(dirPath => Path.GetFullPath(dirPath))
                    .Select(dirPath => Path.Combine(targetRoot, dirPath.Substring(sourceRoot.Length + 1)))
                    .ToList();

                // Create all the directories in parallel
                var options = new ParallelOptions();
                if (Max_Threads > 0)
                {
                    options.MaxDegreeOfParallelism = Max_Threads;
                }
                Parallel.ForEach(directories, options, directoryPath =>
                {
                    Debug.WriteLine("[*] Creating Directory: " + directoryPath);
                    Directory.CreateDirectory(directoryPath);
                });

                // Get a list of all the files to copy
                var files = Directory.GetFiles(Source_Path, "*.*", SearchOption.AllDirectories)
                    .Select(filePath => Path.GetFullPath(filePath))
                    .Select(filePath => new { Source = filePath, Target = Path.Combine(targetRoot, filePath.Substring(sourceRoot.Length + 1)) })
                    .ToList();

                // Copy all the files in parallel
                Parallel.ForEach(files, options, file =>
                {
                    Debug.WriteLine("[*] Copying File: " + file.Source);
                    Console.WriteLine("[*] Copying File: " + file.Source);
                    File.Copy(file.Source, file.Target, true);
                });
            }
            catch (IOException ex)
            {
                Debug.WriteLine("[*] Error copying files: " + ex.Message);
            }
        }


        private String PackageBackup(DirectoryInfo pBackup_Dir)
        {
            Debug.WriteLine("\n\n[*] Packaging Backup");
            Console.WriteLine("\n\n[*] Packaging Backup");
            String Zip_Path = Path.Combine(Backup_Settings.Root_Backup_Dir, DateTime.Now.ToLongDateString());

            ZipFile.CreateFromDirectory(
                pBackup_Dir.FullName,
                Zip_Path,
                CompressionLevel.Optimal,
                false
            );

            if (Backup_Settings.Encrypt) 
            { 
                EncryptionEngine.Encrypt(Zip_Path);
                Zip_Path = Path.ChangeExtension(Zip_Path, "backup");
            }
            else 
            { 
                File.Move(Zip_Path, Path.ChangeExtension(Zip_Path, "zip")); 
            }

            Directory.Delete(pBackup_Dir.FullName, true);

            return Zip_Path;
        }

        public String CreateNewFileBackup()
        {
            DirectoryInfo Backup_Dir = Directory.CreateDirectory(Path.Combine(Backup_Settings.Root_Backup_Dir, "Temp"));

            foreach (string DirPath in Backup_Settings.Backup_Dirs)
            {
                String DirPathName = new DirectoryInfo(DirPath).Name;
                DirectoryInfo DirPathCopyDir = Backup_Dir.CreateSubdirectory(DirPathName);

                Debug.WriteLine("\n\n[*] Backing Up: " + DirPath);
                CopyFiles(DirPath, DirPathCopyDir.FullName);
            }

            return PackageBackup(Backup_Dir);
        }
    }
}
