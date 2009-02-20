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
    public static readonly RoutedCommand PauseJobCommand = new RoutedCommand("PauseJobCommand", typeof(Job));
    public static readonly RoutedCommand StartJobCommand = new RoutedCommand("StartJobCommand", typeof(Job));
    private static Logger logger = LogManager.GetCurrentClassLogger();
    private IWorkItem lastFinishedWI;
    private int totalWorkItemCount;

    public Job()
    {
      this.GatheredWorkItems = new List<IWorkItem>();
      this.FinishedWorkItems = new List<IWorkItem>();
      this.TasksList = new List<ITask>();
      this.IsFinishedVisible = Visibility.Hidden;
    }

    public string Name { get; set; }
    public int Progress { get; set; }
    public List<ITask> TasksList { get; set; }
    public List<IWorkItem> GatheredWorkItems { get; set; }
    public List<IWorkItem> FinishedWorkItems { get; set; }
    public bool IsProcessing { get; set; }
    public bool IsFinished { get; set; }
    public Visibility IsFinishedVisible { get; set; }
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
        logger.Debug("new lastfinished wi: {0}", this.lastFinishedWI.OriginalFile.Name);
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("LastFinishedWI"));
        }
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

      t.ParentJob = this;

      if (this.TasksList.Count == 0) {
        t.Position = TASKPOS.FIRST;
      }
      this.TasksList.Add(t);

      // connect tasks
      if (this.TasksList.Count > 1) {
        this.TasksList[this.TasksList.Count - 2].NextTask = t;
      }

      // tell them their position
      int i = 0;
      foreach (ITask task in this.TasksList) {
        if (i > 0) {
          task.Position = i == this.TasksList.Count - 1 ? TASKPOS.LAST : TASKPOS.MIDDLE;
        }
        i++;
      }

      // listen for enabled
      t.PropertyChanged += new PropertyChangedEventHandler(this.t_PropertyChanged);

      this.Listen4LastEnabledTask();
    }

    private void t_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsEnabled") {
        this.Listen4LastEnabledTask();
      }
    }

    private void Listen4LastEnabledTask()
    {
      // listen to last enabled task
      foreach (ITask task in this.TasksList) {
        task.WIProcessed -= this.task_WIProcessed;
      }
      ITask lastEnabled = this.TasksList.Where(x => x.IsEnabled).Last();
      if (lastEnabled != null) {
        lastEnabled.WIProcessed += this.task_WIProcessed;
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

    private void task_WIProcessed(object sender, TaskEventArgs e)
    {
      // filegathertask will fill workitems
      e.Task.ParentJob.GatheredWorkItems.Add(e.WorkItem);
      // look for last task and add wi to finisheditems list
      if (e.Task.Position == TASKPOS.LAST) {
        this.LastFinishedWI = e.WorkItem;
        this.FinishedWorkItems.Add(e.WorkItem);
      }
      // listen for last wi
      if (e.WorkItem is StopWI) {
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
      return string.Format("{0}:{1} Tasks", this.Name, this.TasksList.Count);
    }
  }
}