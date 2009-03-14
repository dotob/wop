using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WOP.Tasks;

namespace WOP {
  /// <summary>
  /// Interaction logic for JobListTemplate.xaml
  /// </summary>
  public partial class JobListTemplate : UserControl {
    public JobListTemplate()
    {
      this.InitializeComponent();
    }

    private void pausethejob_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null && j.IsProcessing) {
        e.CanExecute = true;
      }
    }

    private void pausethejob_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null) {
        j.Pause();
      }
    }
  }
}