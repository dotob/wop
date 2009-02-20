using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using NLog;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks
{
  public class FileGatherTask : ITask, INotifyPropertyChanged
  {
    #region SORTSTYLE enum

    public enum SORTSTYLE
    {
      NONE,
      FILENAME,
      DATEFILE,
      DATEEXIF
    }

    #endregion

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgWorker = new BackgroundWorker();
    private bool isEnabled;
    private Queue<ImageWI> workItems;

    public FileGatherTask()
    {
      // create bgw
      this.Name = "FileGather-Task";

      this.bgWorker.WorkerReportsProgress = true;
      this.bgWorker.WorkerSupportsCancellation = true;
      this.bgWorker.DoWork += this.bgWorker_DoWork;
      this.DeleteSource = false;
      this.FilePattern = "*";
      this.SortStyles = new ObservableCollection<SORTSTYLE> {SORTSTYLE.NONE, SORTSTYLE.FILENAME, SORTSTYLE.DATEFILE, SORTSTYLE.DATEEXIF};

      this.UI = new FileGatherTaskUI();
      this.UI.DataContext = this;
    }

    public string SourceDirectory { get; set; }
    public string TargetDirectory { get; set; }
    public string FilePattern { get; set; }
    public bool RecurseDirectories { get; set; }
    public bool DeleteSource { get; set; }
    public SORTSTYLE SortOrder { get; set; }
    public ObservableCollection<SORTSTYLE> SortStyles { get; set; }

    #region ITask Members

    public bool IsEnabled
    {
      get { return this.isEnabled; }
      set
      {
        if (this.isEnabled == value) {
          return;
        }
        this.isEnabled = value;
        PropertyChangedEventHandler tmp = this.PropertyChanged;
        if (tmp != null) {
          tmp(this, new PropertyChangedEventArgs("IsEnabled"));
        }
      }
    }

    public Queue<IWorkItem> WorkItems { get; private set; }
    public ITask NextTask { get; set; }
    public string Name { get; set; }

    public UserControl UI { get; set; }
    public Job ParentJob { get; set; }
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

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.gatherFiles();
      this.copyItems();
    }

    private void gatherFiles()
    {
      if (this.workItems == null) {
        // go and gather files 
        logger.Debug("find files with pattern:{0} in sourcedir: {1}, recursedirs:{2}, to targetdir:{3}", this.FilePattern, this.SourceDirectory, this.RecurseDirectories, this.TargetDirectory);
        string[] files = Directory.GetFiles(this.SourceDirectory, this.FilePattern, this.RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        logger.Debug("found {0} files", files.Length);

        // tell job count
        this.ParentJob.TotalWorkItemCount = files.Length;
        // create workitems
        int i = 0;
        var allWI = new List<ImageWI>();
        foreach (string s in files) {
          var fi = new FileInfo(s);
          allWI.Add(new ImageWI(fi) {ProcessPosition = i++});
        }
        // sort them
        logger.Debug("sort by {0}", this.SortOrder);
        switch (this.SortOrder) {
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
        this.workItems = new Queue<ImageWI>();
        foreach (ImageWI wi in allWI) {
          this.workItems.Enqueue(wi);
        }
      }
    }

    private void copyItems()
    {
      foreach (ImageWI wi in this.workItems) {
        if (!this.bgWorker.CancellationPending) {
          string nuFile = Path.Combine(this.TargetDirectory, wi.OriginalFile.Name);
          try {
            File.Copy(wi.CurrentFile.FullName, nuFile, true);
            // set currentfile info
            wi.CurrentFile = new FileInfo(nuFile);
            // do we want to delete?
            if (this.DeleteSource) {
              File.Delete(wi.OriginalFile.FullName);
            }
            // hand them over to next task
            if (this.NextTask != null) {
              lock (this.NextTask.WorkItems) {
                EventHandler<TaskEventArgs> temp = this.WIProcessed;
                if (temp != null) {
                  temp(this, new TaskEventArgs(this, wi));
                }
                this.NextTask.WorkItems.Enqueue(wi);
              }
            }
          }
          catch (Exception ex) {
            logger.ErrorException(string.Format("while copying file {0} to: {1}", wi.OriginalFile.Name, nuFile), ex);
          }
        }
      }
      if (!this.bgWorker.CancellationPending) {
        // remember the stopwi item at the end
        if (this.NextTask != null) {
          lock (this.NextTask.WorkItems) {
            this.NextTask.WorkItems.Enqueue(new StopWI());
          }
        }
      }
    }

    public static int CompareByFileDate(ImageWI a, ImageWI b)
    {
      if (a != null && b != null && a.FileDate != null && b.FileDate != null) {
        return ((DateTime)a.FileDate).CompareTo((DateTime)b.FileDate);
      }
      return 0;
    }

    public static int CompareByExifDate(ImageWI a, ImageWI b)
    {
      if (a != null && b != null && a.ExifDate != null && b.ExifDate != null) {
        return ((DateTime)a.ExifDate).CompareTo((DateTime)b.ExifDate);
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