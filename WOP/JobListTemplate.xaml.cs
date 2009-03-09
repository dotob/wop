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

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null) {
        j.Start();
      }
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null) {
        j.Pause();
      }
    }

    private void XButton_Click(object sender, RoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null) {
        j.DeleteMe();
      }
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

    private void startthejob_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null && !j.IsFinished && !j.IsProcessing) {
        e.CanExecute = true;
      }
    }

    private void startthejob_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      var j = this.DataContext as Job;
      if (j != null) {
        j.Start();
      }
    }
  }
}