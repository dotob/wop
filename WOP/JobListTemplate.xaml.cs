using System.Windows;
using System.Windows.Controls;
using WOP.Tasks;

namespace WOP
{
  /// <summary>
  /// Interaction logic for JobListTemplate.xaml
  /// </summary>
  public partial class JobListTemplate : UserControl
  {
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
  }
}