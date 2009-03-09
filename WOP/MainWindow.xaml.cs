using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using NLog;
using WOP.Objects;
using WOP.Tasks;

namespace WOP {
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    private const int stepsForSplashScreenFadeIn = 50;
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgSplasher = new BackgroundWorker();
    private readonly WOPSplash wopSplash = new WOPSplash();
    private ObservableCollection<Job> jobsToWorkOn = new ObservableCollection<Job>();
    private Job skeletonJob;

    public MainWindow()
    {
      this.Visibility = Visibility.Hidden;
      this.InitializeComponent();
      this.DataContext = this;

      this.bgSplasher.WorkerReportsProgress = true;
      this.bgSplasher.RunWorkerCompleted += this.bgSplasher_RunWorkerCompleted;
      this.bgSplasher.DoWork += this.bgSplasher_DoWork;
      this.bgSplasher.ProgressChanged += this.bgSplasher_ProgressChanged;
      this.wopSplash.Opacity = 0;
      this.wopSplash.Show();
      this.bgSplasher.RunWorkerAsync();
    }

    public ObservableCollection<Job> JobsToWorkOn
    {
      get { return this.jobsToWorkOn; }
      set { this.jobsToWorkOn = value; }
    }

    private void bgSplasher_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.wopSplash.Opacity = 1f/stepsForSplashScreenFadeIn*e.ProgressPercentage;
    }

    private void bgSplasher_DoWork(object sender, DoWorkEventArgs e)
    {
      bool withSplash = false;
      if (withSplash) {
        logger.Debug("show splashscreen");
        for (int i = 0; i < stepsForSplashScreenFadeIn; i++) {
          this.bgSplasher.ReportProgress(i);
          Thread.Sleep(10);
        }
        Thread.Sleep(2000);
        for (int i = stepsForSplashScreenFadeIn/2; i >= 0; i--) {
          this.bgSplasher.ReportProgress(i*2);
          Thread.Sleep(4);
        }
      }
    }

    private void bgSplasher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      this.wopSplash.Close();
      this.Visibility = Visibility.Visible;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.createDefaultJob();
      this.addJobUIs();
    }

    private void createDefaultJob()
    {
      this.skeletonJob = Job.CreateTestJob();
    }

    private void addJobUIs()
    {
      foreach (ITask task in this.skeletonJob.TasksList) {
        if (task.UI != null) {
          task.UI.Margin = new Thickness(2);
          this.m_sp_tasks.Children.Add(task.UI);
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.CloneJobAndAdd();
    }

    private void CloneJobAndAdd()
    {
      Job j = this.skeletonJob.CloneNonDynamicStuff();
      this.JobsToWorkOn.Add(j);
    }

    private void m_mn_whitetheme_Checked(object sender, RoutedEventArgs e)
    {
      if (this.m_mn_whitetheme != null && this.m_mn_blacktheme != null) {
        if (this.m_mn_whitetheme.IsChecked) {
          App.MainApp.ActivateWhiteTheme();
        }
        this.m_mn_blacktheme.IsChecked = false;
      }
    }

    private void m_mn_blacktheme_Checked(object sender, RoutedEventArgs e)
    {
      if (this.m_mn_whitetheme != null && this.m_mn_blacktheme != null) {
        if (this.m_mn_blacktheme.IsChecked) {
          App.MainApp.ActivateBlackTheme();
        }
        this.m_mn_whitetheme.IsChecked = false;
      }
    }

    private void deletethejob_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      FrameworkElement fe = e.OriginalSource as FrameworkElement;
      if (fe != null) {
        Job job = fe.Tag as Job;
        if (job != null) {
          this.jobsToWorkOn.Remove(job);
        }
      }
    }
  }
}