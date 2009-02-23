using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using NLog;
using WOP.Tasks;

namespace WOP
{
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgSplasher = new BackgroundWorker();
    private readonly WOPSplash wopSplash = new WOPSplash();
    private Job theJob;
    private int til = 50;

    public MainWindow()
    {
      this.Visibility = Visibility.Hidden;
      this.InitializeComponent();
      this.bgSplasher.WorkerReportsProgress = true;
      this.bgSplasher.RunWorkerCompleted += this.bgSplasher_RunWorkerCompleted;
      this.bgSplasher.DoWork += this.bgSplasher_DoWork;
      this.bgSplasher.ProgressChanged += this.bgSplasher_ProgressChanged;
      this.wopSplash.Opacity = 0;
      this.wopSplash.Show();
      this.bgSplasher.RunWorkerAsync();
    }

    private void bgSplasher_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.wopSplash.Opacity = 1f / this.til * e.ProgressPercentage;
    }

    private void bgSplasher_DoWork(object sender, DoWorkEventArgs e)
    {
      bool withSplash = true;
      if (withSplash) {
        logger.Debug("show splashscreen");
        for (int i = 0; i < this.til; i++) {
          this.bgSplasher.ReportProgress(i);
          Thread.Sleep(10);
        }
        Thread.Sleep(2000);
        for (int i = this.til / 2; i >= 0; i--) {
          this.bgSplasher.ReportProgress(i * 2);
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
      // configure tasks
      this.theJob = new Job();
      this.theJob.Name = "my first job";
      this.theJob.AddTask(new FileGatherTask {IsEnabled = true, DeleteSource = false, FilePattern = "*.jpg", RecurseDirectories = true, SourceDirectory = @"..\..\..\testdata\pixrotate", TargetDirectory = @"c:\tmp"});
      this.theJob.AddTask(new FileRenamerTask {IsEnabled = true, RenamePattern = "bastitest_{0}"});
      this.theJob.AddTask(new ImageShrinkTask {IsEnabled = true, SizeX = 400, SizeY = 400, PreserveOriginals = true, NameExtension = "_thumb"});
      this.theJob.AddTask(new ImageRotateTask {IsEnabled = false});
      //theJob.AddTask(new FTPTask() { IsEnabled = true, Server = "www.dotob.de", ServerDirectory = "files", UserName = "web1", Password = "celeron" });
      //snootheJob.AddTask(new GEOTagTask { IsEnabled = false });
    }

    private void addJobUIs()
    {
      foreach (ITask task in this.theJob.TasksList) {
        if (task.UI != null) {
          task.UI.Margin = new Thickness(2);
          this.m_sp_tasks.Children.Add(task.UI);
        }
      }
    }

    private void jj_JobFinished(object sender, EventArgs e)
    {
      MessageBox.Show("job finished");
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.m_lb_jobs.Items.Add(this.theJob);
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
  }
}