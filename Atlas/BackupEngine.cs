using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

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
            foreach (string DirPath in Directory.GetDirectories(Source_Path, "*", SearchOption.AllDirectories))
            {
                Debug.WriteLine("[*] Creating Directory: " + DirPath.Replace(Source_Path, Target_Path));
                Directory.CreateDirectory(DirPath.Replace(Source_Path, Target_Path));
            }

            foreach (string OldPath in Directory.GetFiles(Source_Path, "*.*", SearchOption.AllDirectories))
            {
                String NewPath = OldPath.Replace(Source_Path, Target_Path);
                Debug.WriteLine("[*] Copying File: " + OldPath + " To: " + NewPath);
                File.Copy(OldPath, NewPath, true);
            }
        }

        private String PackageBackup(DirectoryInfo pBackup_Dir)
        {
            Debug.WriteLine("\n\n[*] Packaging Backup");
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
