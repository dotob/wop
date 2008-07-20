using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks {
    public class FileGatherTask : ITask {
        #region SORTSTYLE enum

        public enum SORTSTYLE {
            NONE,
            FILENAME,
            DATEFILE,
            DATEEXIF
        }

        #endregion

        private readonly BackgroundWorker bgWorker = new BackgroundWorker();
        // TODO: is this queue thread save??
        private readonly Queue<ImageWI> workItems = new Queue<ImageWI>();

        public FileGatherTask(Job j)
        {
            Parent = j;
            // create bgw
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += bgWorker_DoWork;
        }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public bool RecurseDirectories { get; set; }
        public SORTSTYLE SortOrder { get; set; }

        #region ITask Members

        public Queue<ImageWI> WorkItems { get; private set; }
        public ITask NextTask { get; set; }
        public string Name { get; set; }
        public UserControl UI { get; set; }
        public Job Parent { get; set; }
        public TASKPOS Position { get; set; }
        public Dictionary<ITask, string> TaskInfos { get; set; }

        public event EventHandler<TaskEventArgs> WIProcessed;

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
            // go and gather files 
            // create workitems
            // sort them
            // copy them
            // hand them over to next task
            // remember the stopwi item at the end
        }


        public bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}