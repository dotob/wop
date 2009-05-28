using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NLog;
using WOP.Objects;

namespace WOP.Tasks {
  public class Job : INotifyPropertyChanged {
    public static readonly RoutedCommand DeleteJob = new RoutedCommand("DeleteJobCommand", typeof (Job));
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public static readonly RoutedCommand PauseJob = new RoutedCommand("PauseJobCommand", typeof (Job));
    public static readonly RoutedCommand StartJob = new RoutedCommand("StartJobCommand", typeof (Job));
    private BitmapSource currentWorkItemThumbnail;
    private int finishedWorkItemCount;
    private bool isEnqueued;
    private bool isFinished;
    private Visibility isFinishedVisible;
    private bool isProcessing;
    private IWorkItem lastFinishedWI = new StartWI();
    private string processInfoString;
    private int progress;
    private int totalWorkItemCount;

    public Job()
    {
      this.GatheredWorkItems = new List<IWorkItem>();
      this.FinishedWorkItems = new List<IWorkItem>();
      this.TasksList = new List<ITask>();
      this.IsFinishedVisible = Visibility.Hidden;
    }

    public string Name { get; set; }

    public int Progress
    {
      get { return this.progress; }
      set
      {
        if (this.progress == value) {
          return;
        }
        this.progress = value;
        this.RaisePropertyChangedEvent("Progress");
      }
    }

    public List<ITask> TasksList { get; set; }
    public List<IWorkItem> GatheredWorkItems { get; set; }
    public List<IWorkItem> FinishedWorkItems { get; set; }

    public bool IsProcessing
    {
      get { return this.isProcessing; }
      set
      {
        if (this.isProcessing == value) {
          return;
        }
        this.isProcessing = value;
        this.RaisePropertyChangedEvent("IsProcessing");
      }
    }

    public string ProcessInfoString
    {
      get { return this.processInfoString; }
      set
      {
        if (this.processInfoString == value) {
          return;
        }
        this.processInfoString = value;
        this.RaisePropertyChangedEvent("ProcessInfoString");
      }
    }

    public bool IsFinished
    {
      get { return this.isFinished; }
      set
      {
        if (this.isFinished == value) {
          return;
        }
        this.isFinished = value;
        this.RaisePropertyChangedEvent("IsFinished");
      }
    }

    public bool IsEnqueued
    {
      get { return this.isEnqueued; }
      set
      {
        if (this.isEnqueued == value) {
          return;
        }
        this.isEnqueued = value;
        if (this.isEnqueued) {
          this.ProcessInfoString = string.Format("Warte auf Ausführung...");
        }
        this.RaisePropertyChangedEvent("IsEnqueued");
      }
    }

    public Visibility IsFinishedVisible
    {
      get { return this.isFinishedVisible; }
      set
      {
        if (this.isFinishedVisible == value) {
          return;
        }
        this.isFinishedVisible = value;
        this.RaisePropertyChangedEvent("IsFinishedVisible");
      }
    }

    public bool IsPaused { get; set; }


    public IWorkItem LastFinishedWI
    {
      get { return this.lastFinishedWI; }
      set
      {
        if (this.lastFinishedWI == value) {
          return;
        }
        this.lastFinishedWI = value;
        logger.Debug("new lastfinished wi: {0}", this.lastFinishedWI.Name);
        this.RaisePropertyChangedEvent("LastFinishedWI");
      }
    }

    public int TotalWorkItemCount
    {
      get { return this.totalWorkItemCount; }
      set
      {
        if (this.totalWorkItemCount == value) {
          return;
        }
        this.totalWorkItemCount = value;
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("TotalWorkItemCount"));
        }
      }
    }

    public int FinishedWorkItemCount
    {
      get { return this.finishedWorkItemCount; }
      set
      {
        if (this.finishedWorkItemCount == value) {
          return;
        }
        this.finishedWorkItemCount = value;
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("FinishedWorkItemCount"));
        }
      }
    }

    public BitmapSource CurrentWorkItemThumbnail
    {
      get { return this.currentWorkItemThumbnail; }
      set
      {
        if (this.currentWorkItemThumbnail == value) {
          return;
        }
        this.currentWorkItemThumbnail = value;
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("CurrentWorkItemThumbnail"));
        }
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    private void RaisePropertyChangedEvent(string lastfinishedwi)
    {
      PropertyChangedEventHandler tmp = this.PropertyChanged;
      if (tmp != null) {
        tmp(this, new PropertyChangedEventArgs(lastfinishedwi));
      }
    }

    public event EventHandler JobFinished;

    public void Start()
    {
      this.IsProcessing = true;
      if (this.TasksList != null) {
        logger.Info("start job: {0}", this.Name);
        this.ProcessInfoString = string.Format("Lese Dateien ein...");
        for (int i = this.TasksList.Count - 1; i >= 0; i--) {
          ITask t = this.TasksList[i];
          if (t.IsEnabled) {
            t.Start();
          }
        }
      }
    }

    public void Pause()
    {
      this.IsProcessing = false;
      if (this.TasksList != null) {
        logger.Info("pause job: {0}", this.Name);
        this.ProcessInfoString = string.Format("Pausiere");
        foreach (ITask task in this.TasksList) {
          task.Pause();
        }
      }
    }

    public void HandOverWorkItemToNextEnabledTask(ITask t, IWorkItem wi)
    {
      if (t != null && wi != null) {
        ITask nextTask = null;
        IEnumerable<ITask> enabledTasks = this.TasksList.Where(x => x.IsEnabled);
        bool found = false;
        foreach (ITask task in enabledTasks) {
          if (found) {
            nextTask = task;
            break;
          }
          if (task == t) {
            found = true;
          }
        }
        if (nextTask != null) {
          lock (nextTask.WorkItems) {
            nextTask.WorkItems.Enqueue(wi);
          }
        }
      }
    }

    public void AddTask(ITask t)
    {
      logger.Debug("add task: {0}", t.Name);

      // connect to this
      t.ParentJob = this;

      // set this tasks position
      if (this.TasksList.Count == 0) {
        t.Position = TASKPOS.FIRST;
      } else {
        t.Position = TASKPOS.LAST;
      }
      // tell others tasks their position
      foreach (ITask middleTask in this.TasksList.Where(x => x.Position != TASKPOS.FIRST)) {
        middleTask.Position = TASKPOS.MIDDLE;
      }
      // add the given to this list
      this.TasksList.Add(t);

      // connect tasks, so everyone knows its follower
      if (this.TasksList.Count > 1) {
        this.TasksList[this.TasksList.Count - 2].NextTask = t;
      }

      // listen for en-/disabling
      t.PropertyChanged += new PropertyChangedEventHandler(this.t_PropertyChanged);

      this.Listen4ProcessedWIOfLastEnabledTask();
    }

    private void t_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsEnabled") {
        ITask t = sender as ITask;
        if (t != null) {
          logger.Debug("task {0} IsEnabled set to: {1}", t.Name, t.IsEnabled);
        }
        this.Listen4ProcessedWIOfLastEnabledTask();
      }
    }

    private void Listen4ProcessedWIOfLastEnabledTask()
    {
      // listen to last enabled task
      foreach (ITask task in this.TasksList) {
        task.WIProcessed -= this.lastTaskHasProcessedWI;
      }
      ITask lastEnabled = this.TasksList.Where(x => x.IsEnabled).Last();
      if (lastEnabled != null) {
        logger.Debug("found task {0} to listen to as last enabled", lastEnabled.Name);
        lastEnabled.WIProcessed += this.lastTaskHasProcessedWI;
      }
    }

    public static Job CreateTestJob()
    {
      // configure tasks
      Job skeletonJob = new Job();
      skeletonJob.Name = "my first job";
      skeletonJob.AddTask(new FileGatherTask {IsEnabled = true, DeleteSource = false, FilePattern = "*.jpg", RecurseDirectories = true, SourceDirectory = @"..\..\..\testdata\pixrotate", TargetDirectory = @"c:\tmp"});
      skeletonJob.AddTask(new FileRenamerTask {IsEnabled = true, RenamePattern = "bastitest_{0:000}"});
      skeletonJob.AddTask(new ImageShrinkTask {IsEnabled = true, SizeX = 400, SizeY = 400, PreserveOriginals = true, NameExtension = "_thumb"});
      skeletonJob.AddTask(new DirSorterTask {IsEnabled = true, DirectoryFillCount = 2, DirectoryPattern = "test_{0:000}"});
      skeletonJob.AddTask(new ImageRotateTask {IsEnabled = false});
//      skeletonJob.AddTask(new FTPTask() { IsEnabled = true, Server = "www.dotob.de", ServerDirectory = "files", UserName = "", Password = "" });
//      skeletonJob.AddTask(new GEOTagTask { IsEnabled = false });
      skeletonJob.AddTask(new SliceTask() {IsEnabled = false, XSliceCount = 5, YSliceCount = 5});
      skeletonJob.AddTask(new CleanResourcesTask {IsEnabled = true});
      return skeletonJob;
    }

    private void lastTaskHasProcessedWI(object sender, TaskEventArgs e)
    {
      logger.Info("last task {0} hast processed WI: {1}", e.Task.Name, e.WorkItem.Name);

      this.ProcessInfoString = string.Format("Bearbeite: {0}", e.WorkItem.Name);

      // filegathertask will fill workitems
      // whats this for?????
      e.Task.ParentJob.GatheredWorkItems.Add(e.WorkItem);

      //  add wi to finisheditems list
      if (!(e.WorkItem is StopWI)) {
        this.LastFinishedWI = e.WorkItem;
        this.FinishedWorkItems.Add(e.WorkItem);
        this.FinishedWorkItemCount = this.FinishedWorkItems.Count;
      }

      // set thumb
      var imageWI = e.WorkItem as ImageWI;
      if (imageWI != null) {
        //this.CurrentWorkItemThumbnail = ImageWorker.GetBitmapSourceFromFI(ImageWorker.GetShrinkedDIB(imageWI.ImageHandle, new Size(100,100)));
      }

      // set progress
      this.Progress = (int) ((this.FinishedWorkItems.Count*1f/this.TotalWorkItemCount)*100);

      // listen for last wi
      if (e.WorkItem is StopWI) {
        logger.Info("catched StopWI from task {0}", e.Task.Name, e.WorkItem.Name);
        this.IsFinishedVisible = Visibility.Visible;
        // job seems finished tell it
        EventHandler temp = this.JobFinished;
        if (temp != null) {
          temp(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// works only on jobs that 
    /// </summary>
    /// <returns></returns>
    public Job CloneNonDynamicStuff()
    {
      Job j = new Job();
      j.Name = this.Name;
      foreach (ITask task in this.TasksList) {
        j.AddTask(task.CloneNonDynamicStuff());
      }
      return j;
    }

    public override string ToString()
    {
      return string.Format("{0}:{1} Tasks, WI:{2}/{3}", this.Name, this.TasksList.Count, this.FinishedWorkItems.Count, this.TotalWorkItemCount);
    }

    public void DeleteMe() {}
  }
}