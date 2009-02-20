﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using NLog;
using WOP.Objects;

namespace WOP.Tasks
{
  public abstract class SkeletonTask : ITask, INotifyPropertyChanged
  {
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgWorker = new BackgroundWorker();
    // TODO: is this queue thread save??
    private readonly Queue<IWorkItem> workItems = new Queue<IWorkItem>();
    private bool isEnabled;

    protected SkeletonTask()
    {
      // create bgw
      this.bgWorker.WorkerReportsProgress = true;
      this.bgWorker.WorkerSupportsCancellation = true;
      this.bgWorker.DoWork += this.bgWorker_DoWork;
    }

    public string Name { get; set; }

    public UserControl UI { get; set; }

    #region ITask Members

    public Queue<IWorkItem> WorkItems
    {
      get { return this.workItems; }
    }

    public Dictionary<ITask, string> TaskInfos { get; set; }

    public event EventHandler<TaskEventArgs> WIProcessed;

    public ITask NextTask { get; set; }

    public bool IsEnabled
    {
      get { return this.isEnabled; }
      set
      {
        if (this.isEnabled == value) {
          return;
        }
        this.isEnabled = value;
        this.RaisePropertyChangedEvent("IsEnabled");
      }
    }

    protected void RaisePropertyChangedEvent(string prop)
    {
      PropertyChangedEventHandler tmp = this.PropertyChanged;
      if (tmp != null) {
        tmp(this, new PropertyChangedEventArgs(prop));
      }
    }

    string ITask.Name
    {
      get { return this.Name; }
      set { this.Name = value; }
    }

    UserControl ITask.UI
    {
      get { return this.UI; }
      set { this.UI = value; }
    }

    public Job ParentJob { get; set; }
    public TASKPOS Position { get; set; }

    public void Start()
    {
      // start it
      this.bgWorker.RunWorkerAsync();
    }

    public void Pause()
    {
      this.bgWorker.CancelAsync();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      // infinite loop
      while (true) {
        try {
          if (this.bgWorker.CancellationPending) {
            return;
          }
          // get item from queue and process it
          lock (this.workItems) {
            IWorkItem wi = this.workItems.Dequeue();
            if (wi == null) {
              continue;
            }
            if (wi is ImageWI) {
              var iwi = (ImageWI)wi;
              logger.Info("task {0} start processing: {1}", this.Name, iwi);
              this.Process(iwi);
            }
            // tell job (or anyone else) we have finised process
            this.throwProcessedEvent(wi);
            // add procedd wi into next tasks queue
            if (this.Position != TASKPOS.LAST && this.NextTask != null) {
              lock (this.NextTask.WorkItems) {
                this.NextTask.WorkItems.Enqueue(wi);
              }
            }
            // check if we want to stop
            if (wi is StopWI) {
              // stop
              return;
            }
          }
        }
        catch (InvalidOperationException iex) {
          // notting doto here...queue seems empty
          Thread.Sleep(2000);
        }
      }
    }

    private void throwProcessedEvent(IWorkItem iwi)
    {
      EventHandler<TaskEventArgs> temp = this.WIProcessed;
      if (temp != null) {
        temp(this, new TaskEventArgs(this, iwi));
      }
    }

    public abstract bool Process(ImageWI iwi);
  }
}