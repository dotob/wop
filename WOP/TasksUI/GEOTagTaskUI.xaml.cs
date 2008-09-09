using System.IO;
using System.Windows;
using System.Windows.Controls;
using WOP.Tasks;

namespace WOP.TasksUI {
    /// <summary>
    /// Interaction logic for GEOTagTaskUI.xaml
    /// </summary>
    public partial class GEOTagTaskUI : UserControl {
        public GEOTagTaskUI()
        {
            InitializeComponent();
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            var task = DataContext as GEOTagTask;
            if (task != null) {
                task.GPXFiles.Add(new FileInfo(gpxFile.Text));
            }
        }

        private void delFile_Click(object sender, RoutedEventArgs e)
        {
            var task = DataContext as GEOTagTask;
            if (task != null) {
                var s = gpxFileList.SelectedItem as FileInfo;
                if (s != null) {
                    task.GPXFiles.Remove(s);
                }
            }
        }

        private void browseFile_Click(object sender, RoutedEventArgs e)
        {
            gpxFile.Text = Util.Utils.GetFileFromDialog(".").FullName;
        }
    }
}