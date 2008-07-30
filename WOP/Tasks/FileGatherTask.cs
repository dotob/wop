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
        // TODO: is this queue thread save??
        private readonly Queue<IWorkItem> workItems = new Queue<IWorkItem>();

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
            GatherFiles(this);
        }

        public static void Test()
        {
            var fgt = new FileGatherTask();
            fgt.SourceDirectory = @"..\..\..\IM\pix";
            fgt.TargetDirectory = @"c:\tmp";
            fgt.SortOrder = SORTSTYLE.FILENAME;
            fgt.RecurseDirectories = true;
            fgt.FilePattern = "*jpg";
            GatherFiles(fgt);
        }

        private static void GatherFiles(FileGatherTask gatherTask)
        {
            // go and gather files 
            string[] file = Directory.GetFiles(gatherTask.SourceDirectory, gatherTask.FilePattern, gatherTask.RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            // create workitems
            int i = 0;
            var allWI = new List<ImageWI>();
            foreach (string s in file) {
                var fi = new FileInfo(s);
                var iwi = new ImageWI(fi);
                iwi.ProcessPosition = i++;
                allWI.Add(iwi);
            }
            // sort them
            switch (gatherTask.SortOrder) {
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
            // copy them
            foreach (ImageWI wi in allWI) {
                try {
                    File.Copy(wi.CurrentFile.FullName, Path.Combine(gatherTask.TargetDirectory, wi.OriginalFile.Name), true);
                    // do we want to delete?
                    if (gatherTask.DeleteSource) {
                        File.Delete(wi.OriginalFile.FullName);
                    }
                    // hand them over to next task
                    if (gatherTask.NextTask != null) {
                        gatherTask.NextTask.WorkItems.Enqueue(wi);
                    }
                } catch (Exception ex) {
                    ex.ToString();
                }
            }
            // remember the stopwi item at the end
            if (gatherTask.NextTask != null) {
                gatherTask.NextTask.WorkItems.Enqueue(new StopWI());
            }
        }

        public static int CompareByFileDate(ImageWI a, ImageWI b)
        {
            if (a != null && b != null) {
                return a.FileDate.CompareTo(b.FileDate);
            }
            return 0;
        }

        public static int CompareByExifDate(ImageWI a, ImageWI b)
        {
            if (a != null && b != null) {
                return a.ExifDate.CompareTo(b.ExifDate);
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