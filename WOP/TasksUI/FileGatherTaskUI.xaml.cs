using System.Windows;
using System.Windows.Controls;

namespace WOP.TasksUI
{
  /// <summary>
  /// Interaction logic for FileGatherTaskUI.xaml
  /// </summary>
  public partial class FileGatherTaskUI : UserControl
  {
    public FileGatherTaskUI()
    {
      this.InitializeComponent();
    }

    private void searchTargetDir_Click(object sender, RoutedEventArgs e)
    {
      this.targetDir.Text = Util.Utils.GetDirFromDialog(this.targetDir.Text).FullName;
    }

    private void searchSourceDir_Click(object sender, RoutedEventArgs e)
    {
      this.sourceDir.Text = Util.Utils.GetDirFromDialog(this.sourceDir.Text).FullName;
    }
  }
}