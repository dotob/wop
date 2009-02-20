using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using NLog;
using WOP.Objects;
using System.Linq;

namespace WOP.Tasks
{
  public class Job : INotifyPropertyChanged
  {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public static readonly RoutedCommand PauseJobCommand = new RoutedCommand("PauseJobCommand", typeof(Job));
    public static readonly RoutedCommand StartJobCommand = new RoutedCommand("StartJobCommand", typeof(Job));
    private bool isFinished;
    private Visibility isFinishedVisible;
    private bool isProcessing;
    private IWorkItem lastFinishedWI = new StartWI();
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
      if (this.TasksList != null) {
        logger.Info("start job: {0}", this.Name);
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
      if (this.TasksList != null) {
        logger.Info("pause job: {0}", this.Name);
        foreach (ITask task in this.TasksList) {
          task.Pause();
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
      }
      else {
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
      var j = new Job();
      var fgt = new FileGatherTask();
      fgt.SourceDirectory = @"..\..\..\IM\pixweniger";
      fgt.TargetDirectory = @"c:\tmp";
      fgt.SortOrder = FileGatherTask.SORTSTYLE.FILENAME;
      fgt.RecurseDirectories = true;
      fgt.FilePattern = "*jpg";
      j.AddTask(fgt);

      var frt = new FileRenamerTask();
      frt.RenamePattern = "bastiTest_{0}";
      j.AddTask(frt);

      j.AddTask(new ImageRotateTask());
      j.AddTask(new ImageShrinkTask());

      return j;
    }

    private void lastTaskHasProcessedWI(object sender, TaskEventArgs e)
    {
      logger.Info("last task {0} hast processed WI: {1}", e.Task.Name, e.WorkItem.Name);

      // filegathertask will fill workitems
      e.Task.ParentJob.GatheredWorkItems.Add(e.WorkItem);

      //  add wi to finisheditems list
      this.LastFinishedWI = e.WorkItem;
      this.FinishedWorkItems.Add(e.WorkItem);

      // set progress
      this.Progress = (TotalWorkItemCount / FinishedWorkItems.Count) * 100;

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

    public override string ToString()
    {
      return string.Format("{0}:{1} Tasks, WI:{2}/{3}", this.Name, this.TasksList.Count, this.FinishedWorkItems.Count, this.TotalWorkItemCount);
    }
  }
}