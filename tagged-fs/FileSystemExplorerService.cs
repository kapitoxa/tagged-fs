using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tagged_fs
{
    public class FileSystemExplorerService
    {
        public static IList<DriveInfo> GetRootDirectories()
        {
            return DriveInfo.GetDrives().ToList();
        }

        public static IList<DirectoryInfo> GetChildDirectories(string directory)
        {
            try
            {
                return new DirectoryInfo(directory).GetDirectories().Where(directoryInfo =>
                    !directoryInfo.Attributes.HasFlag(FileAttributes.System) &&
                    !directoryInfo.Attributes.HasFlag(FileAttributes.Temporary) &&
                    !directoryInfo.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }
            catch
            {
                return new List<DirectoryInfo>();
            }
        }

        public static IList<FileInfo> GetChildFiles(string directory)
        {
            try
            {
                return new DirectoryInfo(directory).GetFiles().Where(fileInfo =>
                    fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Temporary) &&
                    !fileInfo.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
            }
            catch
            {
                return new List<FileInfo>();
            }
        }
    }
}
