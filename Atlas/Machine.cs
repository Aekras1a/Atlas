using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Atlas
{
    internal class Machine
    {
        private readonly String drive;
        private readonly String processorId;

        public readonly long totalDriveSpace;
        public readonly long totalUsedDriveSpace;

        public String hardwareId = null;

        public Machine()
        {
            Debug.WriteLine($"[*] ({this.GetType().Name}) Getting Hardware Id");
            drive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);
            processorId = GetProcessorId();

            totalDriveSpace = GetTotalDriveSpace(false);
            totalUsedDriveSpace = GetTotalDriveSpace(true);

            hardwareId = GetHardwareId().ToString();
        }

        private Guid GetHardwareId()
        {
            String volumeSerial = GetVolumeSerialNumber();

            String uuid = $"{volumeSerial}-{processorId}";

            byte[] hashBytes;
            using (var sha256 = SHA256.Create())
            {
                byte[] uuidBytes = Encoding.UTF8.GetBytes(uuid);
                hashBytes = sha256.ComputeHash(uuidBytes);
            }

            return new Guid(hashBytes.Take(16).ToArray());
        }

        private String GetVolumeSerialNumber()
        {
            try
            {
                ManagementObject disk = new ManagementObject($"win32_logicaldisk.deviceid=\"{drive}\"");
                disk.Get();
                return disk["VolumeSerialNumber"].ToString();
            }
            catch (Exception ex)
            {
                throw new HardwareIdException("Failed to retrieve volume serial number.", ex);
            }
        }

        private String GetProcessorId()
        {
            try
            {
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                ManagementObject mo = moc.Cast<ManagementObject>().FirstOrDefault();
                return mo.Properties["ProcessorId"].Value.ToString();
            }
            catch (Exception ex)
            {
                throw new HardwareIdException("Failed to retrieve processor ID.", ex);
            }
        }

        private long GetTotalDriveSpace(bool usedSpace)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            long totalSpace = 0;

            foreach (DriveInfo drive in allDrives)
            {
                try
                {
                    if (drive.IsReady)
                    {
                        totalSpace += usedSpace ? (drive.TotalSize - drive.AvailableFreeSpace) : drive.TotalSize;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting drive information: {ex.Message}");
                }
            }

            double totalSpaceTB = Math.Round((double)totalSpace / 1000000000000, 2);
            Debug.WriteLine($"[*] ({this.GetType().Name} Total {(usedSpace ? "used" : "available")} space of all drives: {totalSpaceTB} TB");
            return totalSpace;
        }
    }

    public class HardwareIdException : Exception
    {
        public HardwareIdException(String message, Exception innerException) : base(message, innerException)
        {
            Debug.WriteLine($"[-] ({this.GetType().Name}) {message} -> {innerException}");
        }
    }
}