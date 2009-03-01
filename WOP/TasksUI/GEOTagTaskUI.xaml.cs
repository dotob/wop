using System.IO;
using System.Windows;
using System.Windows.Controls;
using WOP.Tasks;

namespace WOP.TasksUI
{
  /// <summary>
  /// Interaction logic for GEOTagTaskUI.xaml
  /// </summary>
  public partial class GEOTagTaskUI : UserControl
  {
    public GEOTagTaskUI()
    {
      this.InitializeComponent();
    }

    private void addFile_Click(object sender, RoutedEventArgs e)
    {
      var task = this.DataContext as GEOTagTask;
      if (task != null) {
        task.GpxFiles.Add(new FileInfo(this.gpxFile.Text));
      }
    }

    private void delFile_Click(object sender, RoutedEventArgs e)
    {
      var task = this.DataContext as GEOTagTask;
      if (task != null) {
        var s = this.gpxFileList.SelectedItem as FileInfo;
        if (s != null) {
          task.GpxFiles.Remove(s);
        }
      }
    }

    private void browseFile_Click(object sender, RoutedEventArgs e)
    {
      this.gpxFile.Text = Util.Utils.GetFileFromDialog(".").FullName;
    }
  }
}