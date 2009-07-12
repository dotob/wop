using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NLog;
using WOP.Objects;

namespace WOP.Tasks {
  public abstract class SkeletonTask : ITask, INotifyPropertyChanged {
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgWorker = new BackgroundWorker();
    // TODO: is this queue thread save??
    private readonly Queue<IWorkItem> workItems = new Queue<IWorkItem>();
    private bool isEnabled;
    private TASKWORKINGSTYLE workingStyle =TASKWORKINGSTYLE.STRAIGHT;

    protected SkeletonTask()
    {
      // create bgw
      this.bgWorker.WorkerReportsProgress = true;
      this.bgWorker.WorkerSupportsCancellation = true;
      this.bgWorker.DoWork += this.bgWorker_DoWork;
      this.bgWorker.RunWorkerCompleted += this.bgWorker_RunWorkerCompleted;
    }

    public Dictionary<ITask, string> TaskInfos { get; set; }

    public Type TaskType
    {
      get { return this.GetType(); }
    }

    #region ITask Members

    public abstract UserControl UI { get; set; }
    public string Name { get; protected set; }

    public Queue<IWorkItem> WorkItems
    {
      get { return this.workItems; }
    }

    public TASKWORKINGSTYLE WorkingStyle
    {
      get { return this.workingStyle; }
      set
      {
        if (this.workingStyle == value) {
          return;
        }
        this.workingStyle = value;
        this.RaisePropertyChangedEvent("WorkingStyle");
      }
    }

    public virtual TASKWORKINGSTYLE WorkingStyleConstraint
    {
      get { return TASKWORKINGSTYLE.ALL; }
    }


    public virtual Visibility UIVisibility
    {
      get { return Visibility.Visible; }
    }

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

    public bool CanChangePosition
    {
      get { return true; }
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

    public abstract ITask CloneNonDynamicStuff();

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null) {
        logger.Error(e.Error);
        MessageBox.Show(e.Error.ToString(), "Error in JobQueue");
      }
    }

    protected void RaisePropertyChangedEvent(string prop)
    {
      PropertyChangedEventHandler tmp = this.PropertyChanged;
      if (tmp != null) {
        tmp(this, new PropertyChangedEventArgs(prop));
      }
    }

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
              var iwi = (ImageWI) wi;
              logger.Info("task {0} start processing: {1}", this.Name, iwi);
              this.Process(iwi);
            }
            // tell job (or anyone else) we have finised process
            this.throwProcessedEvent(wi);
            // add processed wi into next tasks queue
            if (this.ParentJob != null) {
              this.ParentJob.HandOverWorkItemToNextEnabledTask(this, wi);
            }
            // check if we want to stop
            if (wi is StopWI) {
              // stop
              return;
            }
          }
        } catch (InvalidOperationException iex) {
          // notting todo here...queue seems empty
          logger.Trace("{0} is waiting for its predecessor", this.Name);
          Thread.Sleep(2000);
        } catch (Exception ex) {
          logger.Error("{0} catched unexcepted exception: {1}", this.Name, ex);
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