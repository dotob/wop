using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using NLog;
using WOP.Objects;

namespace WOP.Tasks {
  //[JsonConverter(typeof(JSONTaskConverter))]
  public abstract class SkeletonTask : ITask, INotifyPropertyChanged {
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
      this.bgWorker.RunWorkerCompleted += this.bgWorker_RunWorkerCompleted;
    }

    void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if(e.Error!=null) {
        logger.Error(e.Error);
        MessageBox.Show(e.Error.ToString(), "Error in JobQueue");
      }
    }

    public abstract UserControl UI { get; set; }

    public Dictionary<ITask, string> TaskInfos { get; set; }

    #region ITask Members

    [JsonProperty]
    public string Name { get; protected set; }

    [JsonProperty]
    public Type TaskType
    {
      get
      {
        return this.GetType();
      }
    }

    public Queue<IWorkItem> WorkItems
    {
      get { return this.workItems; }
    }

    public event EventHandler<TaskEventArgs> WIProcessed;

    public ITask NextTask { get; set; }

    [JsonProperty]
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
        } catch(Exception ex) {
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

  internal class JSONTaskConverter:JsonConverter {
    public override void WriteJson(JsonWriter writer, object value)
    {
      
    }

    public override object ReadJson(JsonReader reader, Type objectType)
    {
      return string.Empty;
    }

    public override bool CanConvert(Type objectType)
    {
      return true;
    }
  }
}