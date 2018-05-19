using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using tagged_fs.Annotations;

namespace tagged_fs
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
//            System.Threading.Thread.CurrentThread.CurrentUICulture =
//                System.Globalization.CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            DataContext = this;
            FsTreeView.Items.Add(new FileSystemObject(Properties.Resources.Computer) {Type = (int) ObjectType.Computer});
            FsTreeView.Items.Add(new FileSystemObject(Properties.Resources.Workspace) {Type = (int) ObjectType.Workspace});
            TemplateType = -1;
        }

        #region Events

        private void TreeViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var currentNode = sender as TreeViewItem;
            ExplorerView.Items.Clear();

            if (!(currentNode?.Header is FileSystemObject parent))
            {
                return;
            }

            TemplateType = parent.Type == (int) ObjectType.Computer ? 0 : 1;

            if (currentNode.ItemsSource == null)
            {
                return;
            }

            if (parent.Type == (int) ObjectType.Computer)
            {
                foreach (var rootDirectory in FileSystemExplorerService.GetRootDirectories())
                {
                    ExplorerView.Items.Add(new FileSystemObject(rootDirectory));
                }
            }
            else
            {
                foreach (var childDirectory in FileSystemExplorerService.GetChildDirectories(parent.Path))
                {
                    ExplorerView.Items.Add(new FileSystemObject(childDirectory));
                }

                foreach (var childFile in FileSystemExplorerService.GetChildFiles(parent.Path))
                {
                    ExplorerView.Items.Add(new FileSystemObject(childFile));
                }
            }
        }

        private void ListViewItem_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            
            if (!(sender is ListViewItem currentNode))
            {
                return;
            }

            if (!(currentNode.Content is FileSystemObject content))
            {
                return;
            }

            TemplateType = content.Type == (int) ObjectType.Computer ? 0 : 1;

            if (content.Type == (int) ObjectType.Directory || content.Type == (int) ObjectType.Drive)
            {
                ExplorerView.Items.Clear();

                foreach (var childDirectory in FileSystemExplorerService.GetChildDirectories(content.Path))
                {
                    ExplorerView.Items.Add(new FileSystemObject(childDirectory));
                }

                foreach (var childFile in FileSystemExplorerService.GetChildFiles(content.Path))
                {
                    ExplorerView.Items.Add(new FileSystemObject(childFile));
                }
            }
            else if (content.Type == (int) ObjectType.File)
            {
                Process.Start(@content.Path);
            }
        }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private int _templateType;

        public int TemplateType
        {
            get => _templateType;
            set
            {
                if (_templateType == value)
                {
                    return;
                }
                
                _templateType = value;
                OnPropertyChanged(nameof(TemplateType));
            }
        }
    }
}
