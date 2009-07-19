using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using NLog;
using Spring.Context;
using Spring.Context.Support;
using WOP.Objects;
using WOP.Tasks;

namespace WOP {
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged {
    private const int stepsForSplashScreenFadeIn = 50;
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgSplasher = new BackgroundWorker();
    private readonly WOPSplash wopSplash = new WOPSplash();
    private bool executeSequential = true;
    private Queue<Job> jobQueue = new Queue<Job>();
    private ObservableCollection<Job> jobsToWorkOn = new ObservableCollection<Job>();
    private Job processingJob;
    private Job skeletonJob;
    private IApplicationContext springContext;

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

      this.springContext = ContextRegistry.GetContext();
    }

    public ObservableCollection<Job> JobsToWorkOn
    {
      get { return this.jobsToWorkOn; }
      set { this.jobsToWorkOn = value; }
    }

    public bool ExecuteSequential
    {
      get { return this.executeSequential; }
      set
      {
        if (this.executeSequential == value) {
          return;
        }
        this.executeSequential = value;
        this.RaisePropertyChangedEvent("ExecuteSequential");
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

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
      this.skeletonJob = (Job) this.springContext["defaultjob"];
    }

    private void addJobUIs()
    {
      this.m_sp_tasks.ItemsSource = this.skeletonJob.TasksList;
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

    private void startthejob_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      FrameworkElement fe = e.OriginalSource as FrameworkElement;
      if (fe != null) {
        var j = fe.DataContext as Job;
        // only a not processing, not enqueued and not finished job can be started
        if (j != null && !j.IsFinished && !j.IsProcessing && !j.IsEnqueued) {
          e.CanExecute = true;
        }
      }
    }

    private void startthejob_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      FrameworkElement fe = e.OriginalSource as FrameworkElement;
      if (fe != null) {
        var j = fe.DataContext as Job;
        if (j != null) {
          if (this.ExecuteSequential) {
            // queue job...
            this.jobQueue.Enqueue(j);
            j.IsEnqueued = true;
            j.JobFinished += this.whenJobIsFinished;
            this.removeJobAndStartNextJobInQueue();
          } else {
            j.Start();
          }
        }
      }
    }

    private void whenJobIsFinished(object sender, EventArgs e)
    {
      this.processingJob = null;
      this.removeJobAndStartNextJobInQueue();
    }

    private void removeJobAndStartNextJobInQueue()
    {
      if (this.processingJob == null) {
        try {
          Job j = this.jobQueue.Dequeue();
          this.processingJob = j;
          logger.Info("start job {0} is first in queue and none is processing", j);
          j.Start();
        } catch (Exception) {
          logger.Info("no next job to start");
        }
      } else {
        logger.Info("job enqueued. there is still an active Job");
      }
    }

    private void RaisePropertyChangedEvent(string lastfinishedwi)
    {
      PropertyChangedEventHandler tmp = this.PropertyChanged;
      if (tmp != null) {
        tmp(this, new PropertyChangedEventArgs(lastfinishedwi));
      }
    }

    private void SequentialButtonChecked(object sender, RoutedEventArgs e)
    {
      this.ExecuteSequential = true;
    }

    private void SequentialButtonUnChecked(object sender, RoutedEventArgs e)
    {
      this.ExecuteSequential = false;
    }
  }
}