using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using NLog;
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

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private readonly BackgroundWorker bgWorker = new BackgroundWorker();
    private bool isEnabled;
    private TASKPOS position;
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

    [SettingProperty]
    public string SourceDirectory { get; set; }

    [SettingProperty]
    public string TargetDirectory { get; set; }

    [SettingProperty]
    public string FilePattern { get; set; }

    [SettingProperty]
    public bool RecurseDirectories { get; set; }

    [SettingProperty]
    public bool DeleteSource { get; set; }

    [SettingProperty]
    public SORTSTYLE SortOrder { get; set; }

    /// for binding a list to it...
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
    public string Name { get; private set; }

    public UserControl UI { get; set; }
    public Job ParentJob { get; set; }

    /// <summary>
    /// this task can only bee at first position
    /// </summary>
    public TASKPOS Position
    {
      get { return this.position; }
      set
      {
        if (value != TASKPOS.FIRST) {
          throw new ArgumentException("Der FileGatherTask kann nur an erster Stelle eines Jobs kommen.", "Position");
        }
        this.position = value;
      }
    }

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

    public ITask CloneNonDynamicStuff()
    {
      FileGatherTask t = new FileGatherTask();
      t.IsEnabled = this.IsEnabled;
      t.SourceDirectory = this.SourceDirectory;
      t.TargetDirectory = this.TargetDirectory;
      t.RecurseDirectories = this.RecurseDirectories;
      t.SortOrder = this.SortOrder;
      t.FilePattern = this.FilePattern;
      t.DeleteSource = this.DeleteSource;
      return t;
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
            // tell any interest that we processed a wi
            EventHandler<TaskEventArgs> temp = this.WIProcessed;
            if (temp != null) {
              temp(this, new TaskEventArgs(this, wi));
            }
            this.ParentJob.HandOverWorkItemToNextEnabledTask(this, wi);
          } catch (Exception ex) {
            logger.ErrorException(string.Format("error while copying file {0} to: {1}", wi.OriginalFile.Name, nuFile), ex);
          }
        }
      }
      if (!this.bgWorker.CancellationPending) {
        // remember the stopwi item at the end
        this.ParentJob.HandOverWorkItemToNextEnabledTask(this, new StopWI());
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