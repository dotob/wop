using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks {
    public abstract class SkeletonTask : ITask, INotifyPropertyChanged {
        private readonly BackgroundWorker bgWorker = new BackgroundWorker();
        // TODO: is this queue thread save??
        private readonly Queue<IWorkItem> workItems = new Queue<IWorkItem>();

        protected SkeletonTask()
        {
            // create bgw
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += bgWorker_DoWork;
        }

        public string Name { get; set; }

        public UserControl UI { get; set; }

        #region ITask Members

        public Queue<IWorkItem> WorkItems
        {
            get { return workItems; }
        }

        public Dictionary<ITask, string> TaskInfos { get; set; }

        public event EventHandler<TaskEventArgs> WIProcessed;

        public ITask NextTask { get; set; }
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if (this.isEnabled == value)
                {
                    return;
                }
                this.isEnabled = value;
                PropertyChangedEventHandler tmp = this.PropertyChanged;
                if (tmp != null)
                {
                    tmp(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
        }

        string ITask.Name
        {
            get { return Name; }
            set { Name = value; }
        }

        UserControl ITask.UI
        {
            get { return UI; }
            set { UI = value; }
        }

        public Job ParentJob { get; set; }
        public TASKPOS Position { get; set; }

        public void Start()
        {
            // start it
            bgWorker.RunWorkerAsync();
        }

        public void Pause()
        {
            bgWorker.CancelAsync();
        }

        #endregion

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // infinite loop
            while (true) {
                try {
                    if(bgWorker.CancellationPending) {
                        return;
                    }
                    // get item from queue and process it
                    lock (workItems) {
                        IWorkItem wi = workItems.Dequeue();
                        if (wi == null) {
                            continue;
                        }
                        if (wi is ImageWI) {
                            var iwi = (ImageWI) wi;
                            Process(iwi);
                        }
                        // tell job (or anyone else) we have finised process
                        throwProcessedEvent(wi);
                        // add procedd wi into next tasks queue
                        if (Position != TASKPOS.LAST && NextTask != null) {
                            lock (NextTask.WorkItems) {
                                NextTask.WorkItems.Enqueue(wi);
                            }
                        }
                        // check if we want to stop
                        if (wi is StopWI) {
                            // stop
                            return;
                        }
                    }
                } catch (InvalidOperationException iex) {
                    // notting doto here...queue seems empty
                    Thread.Sleep(2000);
                }
            }
        }

        private void throwProcessedEvent(IWorkItem iwi)
        {
            EventHandler<TaskEventArgs> temp = WIProcessed;
            if (temp != null) {
                temp(this, new TaskEventArgs(this, iwi));
            }
        }

        public abstract bool Process(ImageWI iwi);
        public event PropertyChangedEventHandler PropertyChanged;
    }
}