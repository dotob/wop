using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WOP.Objects;
using System.Linq;

namespace WOP.Tasks
{
  public class Job : INotifyPropertyChanged
  {
    public static readonly RoutedCommand PauseJobCommand = new RoutedCommand("PauseJobCommand", typeof(Job));
    public static readonly RoutedCommand StartJobCommand = new RoutedCommand("StartJobCommand", typeof(Job));
    private int workItemCount;

    public Job()
    {
      this.WorkItems = new List<IWorkItem>();
      this.TasksList = new List<ITask>();
      this.IsFinishedVisible = Visibility.Hidden;
    }

    public string Name { get; set; }
    public int Progress { get; set; }
    public List<ITask> TasksList { get; set; }
    public List<IWorkItem> WorkItems { get; set; }
    public bool IsProcessing { get; set; }
    public bool IsFinished { get; set; }
    public Visibility IsFinishedVisible { get; set; }
    public bool IsPaused { get; set; }

    public int WorkItemCount
    {
      get { return this.workItemCount; }
      set
      {
        if (this.workItemCount == value) {
          return;
        }
        this.workItemCount = value;
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("WorkItemCount"));
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
        foreach (ITask task in this.TasksList) {
          task.Pause();
        }
      }
    }

    public void AddTask(ITask t)
    {
      t.Parent = this;

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
      e.Task.Parent.WorkItems.Add(e.WorkItem);
      //e.Task.Parent.WorkItemCount = e.Task.Parent.WorkItems.Count;
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