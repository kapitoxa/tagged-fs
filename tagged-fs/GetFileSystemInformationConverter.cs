using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace tagged_fs
{
    public class GetFileSystemInformationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is FileSystemObject objectToExpand))
                {
                    return null;
                }

                if (objectToExpand.Type == (int) ObjectType.Computer)
                {
                    return FileSystemExplorerService.GetRootDirectories()
                        .Select(driveInfo => new FileSystemObject(driveInfo)).ToList();
                }
                else if (objectToExpand.Type == (int) ObjectType.Workspace)
                {
                    return null;
                }
                else
                {
                    return FileSystemExplorerService.GetChildDirectories(objectToExpand.Path)
                        .Select(directoryInfo => new FileSystemObject(directoryInfo)).ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
