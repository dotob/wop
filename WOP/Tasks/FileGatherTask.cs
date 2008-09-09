using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using WOP.Objects;
using WOP.TasksUI;

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
        private Queue<ImageWI> workItems;

        public FileGatherTask()
        {
            // create bgw
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += bgWorker_DoWork;
            DeleteSource = false;
            FilePattern = "*";
            UI = new FileGatherTaskUI();
            UI.DataContext = this;
        }

        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string FilePattern { get; set; }
        public bool RecurseDirectories { get; set; }
        public bool DeleteSource { get; set; }
        public SORTSTYLE SortOrder { get; set; }

        #region ITask Members

        public bool IsEnabled { get; set; }

        public Queue<IWorkItem> WorkItems { get; private set; }
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
            bgWorker.RunWorkerAsync();
        }

        public void Pause()
        {
            bgWorker.CancelAsync();
        }

        #endregion

        public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            gatherFiles();
            copyItems();
        }

        private void gatherFiles()
        {
            if (workItems == null) {
                // go and gather files 
                string[] files = Directory.GetFiles(SourceDirectory, FilePattern, RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                // tell job count
                Parent.WorkItemCount = files.Length;
                // create workitems
                int i = 0;
                var allWI = new List<ImageWI>();
                foreach (string s in files) {
                    var fi = new FileInfo(s);
                    allWI.Add(new ImageWI(fi) {ProcessPosition = i++});
                }
                // sort them
                switch (SortOrder) {
                    case SORTSTYLE.NONE:
                        break;
                    case SORTSTYLE.FILENAME:
                        allWI.Sort(CompareByFileName);
                        break;
                    case SORTSTYLE.DATEFILE:
                        allWI.Sort(CompareByFileDate);
                        break;
                    case SORTSTYLE.DATEEXIF:
                        allWI.Sort(CompareByExifDate);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                // copy them to queue
                workItems = new Queue<ImageWI>();
                foreach (ImageWI wi in allWI) {
                    workItems.Enqueue(wi);
                }
            }
        }

        private void copyItems()
        {
            foreach (ImageWI wi in workItems) {
                if (!bgWorker.CancellationPending) {
                    try {
                        string nuFile = Path.Combine(TargetDirectory, wi.OriginalFile.Name);
                        File.Copy(wi.CurrentFile.FullName, nuFile, true);
                        // set currentfile info
                        wi.CurrentFile = new FileInfo(nuFile);
                        // do we want to delete?
                        if (DeleteSource) {
                            File.Delete(wi.OriginalFile.FullName);
                        }
                        // hand them over to next task
                        if (NextTask != null) {
                            lock (NextTask.WorkItems) {
                                EventHandler<TaskEventArgs> temp = WIProcessed;
                                if (temp != null) {
                                    temp(this, new TaskEventArgs(this, wi));
                                }
                                NextTask.WorkItems.Enqueue(wi);
                            }
                        }
                    } catch (Exception ex) {
                        ex.ToString();
                    }
                }
            }
            if (!bgWorker.CancellationPending) {
                // remember the stopwi item at the end
                if (NextTask != null) {
                    lock (NextTask.WorkItems) {
                        NextTask.WorkItems.Enqueue(new StopWI());
                    }
                }
            }
        }

        public static int CompareByFileDate(ImageWI a, ImageWI b)
        {
            if (a != null && b != null && a.FileDate != null && b.FileDate != null) {
                return ((DateTime) a.FileDate).CompareTo((DateTime) b.FileDate);
            }
            return 0;
        }

        public static int CompareByExifDate(ImageWI a, ImageWI b)
        {
            if (a != null && b != null && a.ExifDate != null && b.ExifDate != null) {
                return ((DateTime) a.ExifDate).CompareTo((DateTime) b.ExifDate);
            }
            return 0;
        }

        public static int CompareByFileName(ImageWI a, ImageWI b)
        {
            if (a != null && b != null) {
                return a.OriginalFile.Name.CompareTo(b.OriginalFile.Name);
            }
            return 0;
        }

        public bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}