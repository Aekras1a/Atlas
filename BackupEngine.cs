using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Atlas
{
    internal class BackupEngine
    {
        Settings Backup_Settings = null;
        public BackupEngine(Settings pBackup_Settings) 
        {
            Backup_Settings = pBackup_Settings;
        }
        EncryptionEngine EncryptionEngine = new EncryptionEngine();
        
        private void CopyFilesRecursively(string Source_Path, string Target_Path)
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

                if(Backup_Settings.Encrypt) { EncryptionEngine.AES_Encrypt(NewPath, Backup_Settings.EncryptionPassword); }
                
            }
        }

        public void CreateNewFileBackup()
        {
            DirectoryInfo Backup_Dir = Directory.CreateDirectory(Path.Combine(Backup_Settings.Root_Backup_Dir, "Temp"));

            foreach (string DirPath in Backup_Settings.Backup_Dirs)
            {
                String DirPathName = new DirectoryInfo(DirPath).Name;
                DirectoryInfo DirPathCopyDir = Backup_Dir.CreateSubdirectory(DirPathName);

                Debug.WriteLine("\n\n[*] Backing Up: " + DirPath);
                CopyFilesRecursively(DirPath, DirPathCopyDir.FullName);

                ZipFile.CreateFromDirectory(
                    Backup_Dir.FullName, 
                    Path.Combine(
                        Backup_Settings.Root_Backup_Dir, 
                        System.DateTime.Now.ToLongDateString() + ".zip"
                    ), 
                    CompressionLevel.Optimal, 
                    false
                );

                Directory.Delete(Backup_Dir.FullName, true);
            }
        }
    }
}
