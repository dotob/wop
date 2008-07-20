using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks {
    public abstract class SkeletonTask : ITask {
        private readonly BackgroundWorker bgWorker = new BackgroundWorker();
        // TODO: is this queue thread save??
        private readonly Queue<IWorkItem> workItems = new Queue<IWorkItem>();

        protected SkeletonTask()
        {
            // create bgw
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += bgWorker_DoWork;
        }

        protected SkeletonTask(Job parent)
        {
            Parent = parent;
        }

        public string Name { get; private set; }

        public UserControl UI { get; private set; }

        #region ITask Members

        public Queue<IWorkItem> WorkItems
        {
            get { return this.workItems; }
        }

        public Dictionary<ITask, string> TaskInfos { get; set; }

        public event EventHandler<TaskEventArgs> WIProcessed;

        public ITask NextTask { get; set; }

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

        public Job Parent { get; set; }
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

        #endregion

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // infinite loop
            while (true) {
                try {
                    // get item from queue and process it
                    IWorkItem wi = this.workItems.Dequeue();
                    if (wi == null) {
                        continue;
                    }
                    // check if we want to stop
                    if (wi is StopWI) {
                        // stop
                        return;
                    }
                    if (!(wi is ImageWI)) {
                        continue;
                    }
                    var iwi = (ImageWI) wi;
                    Process(iwi);
                    // tell job (or anyone else) we have finised process
                    EventHandler<TaskEventArgs> temp = WIProcessed;
                    if (temp != null) {
                        temp(this, new TaskEventArgs(this, iwi));
                    }
                    // add procedd wi into next tasks queue
                    NextTask.WorkItems.Enqueue(iwi);
                } catch (InvalidOperationException iex) {
                    // notting doto here...queue seems empty
                }
            }
        }

        public abstract bool Process(ImageWI iwi);
    }
}