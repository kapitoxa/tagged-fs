using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace tagged_fs
{
    public enum ObjectType
    {
        Computer = 0,
        Drive = 1,
        Directory = 2,
        File = 3,
        Workspace = 4,
        Tag = 5,
    }

    public class FileSystemObject : DependencyObject
    {
        #region Public Properties

        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string Root { get; set; }
        public string Modified { get; set; }
        public string Extension { get; set; }
        public string TotalSize { get; set; }
        public string FreeSpace { get; set; }
        public int Type { get; set; }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty PropertyChildren = DependencyProperty.Register("Children", typeof(IList<FileSystemObject>), typeof(FileSystemObject));

        public List<FileSystemObject> SubDirectories
        {
            get => (List<FileSystemObject>) GetValue(PropertyChildren);
            set => SetValue(PropertyChildren, value);
        }

        public static readonly DependencyProperty PropertyIsSelected = DependencyProperty.Register("IsSelected", typeof(bool), typeof(FileSystemObject));

        public bool IsSelected
        {
            get => (bool) GetValue(PropertyIsSelected);
            set => SetValue(PropertyIsSelected, value);
        }

        public static readonly DependencyProperty PropertyIsExpanded = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(FileSystemObject));

        public bool IsExpanded
        {
            get => (bool) GetValue(PropertyIsExpanded);
            set => SetValue(PropertyIsExpanded, value);
        }

        #endregion
        
        #region Constructors

        public FileSystemObject()
        {
            SubDirectories = new List<FileSystemObject>();
        }

        public FileSystemObject(string name) : this()
        {
            Name = name;
        }

        public FileSystemObject(DriveInfo driveInfo) : this()
        {
            var driveName = driveInfo.Name;
            var volumeLabel = driveInfo.VolumeLabel;

            Name = (volumeLabel.Length == 0 ? Properties.Resources.DefaultDiskLabel : volumeLabel) + " (" + (driveName.EndsWith(@"\")
                       ? driveName.Substring(0, driveName.Length - 1)
                       : driveName) + ")";
            Path = driveName;
            Type = (int) ObjectType.Drive;
            TotalSize = driveInfo.TotalSize / (1024 * 1024 * 1024) + " " + Properties.Resources.Size_GB;
            FreeSpace = driveInfo.TotalFreeSpace / (1024 * 1024 * 1024) + " " + Properties.Resources.Size_GB;
            SubDirectories.Add(new FileSystemObject("TempDir"));
        }

        public FileSystemObject(DirectoryInfo directoryInfo) : this()
        {
            Name = directoryInfo.Name;
            Root = directoryInfo.Root.Name;
            Path = directoryInfo.FullName;
            Extension = Properties.Resources.FolderExtension;
            Modified = directoryInfo.LastWriteTime.ToShortDateString() + " " +
                       directoryInfo.LastWriteTime.ToShortTimeString();
            
            Type = (int) ObjectType.Directory;
            SubDirectories.Add(new FileSystemObject("TempDir"));

        }

        public FileSystemObject(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            Path = fileInfo.FullName;
            Size = fileInfo.Length / 1024 + " " + Properties.Resources.Size_KB;
            var extension = fileInfo.Extension;
            Extension = (extension.StartsWith(".") ? extension.Substring(1, extension.Length - 1) : extension) + " file";
            Modified = fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString();
            Type = (int) ObjectType.File;
        }

        #endregion
    }
}
