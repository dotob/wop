﻿using System;
using System.IO;
using System.Windows.Controls;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
  public class FileRenamerTask : SkeletonTask {
    private UserControl ui;

    public FileRenamerTask()
    {
      this.Name = "Umbenennen";
    }

    public override UserControl UI
    {
      get
      {
        this.ui = new FileRenamerTaskUI();
        this.ui.DataContext = this;
        return this.ui;
      }
      set { this.ui = value; }
    }

    public string RenamePattern { get; set; }

    public override TASKWORKINGSTYLE WorkingStyleConstraint
    {
      get
      {
        return TASKWORKINGSTYLE.ALL;
      }
    }

    public override ITask CloneNonDynamicStuff()
    {
      FileRenamerTask t = new FileRenamerTask();
      t.IsEnabled = this.IsEnabled;
      t.RenamePattern = this.RenamePattern;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      logger.Info("task {0} start processing: {1}", this.Name, iwi);
      bool success = false;
      string nuName = Path.Combine(iwi.CurrentFile.DirectoryName ?? string.Empty, this.RenamedString(iwi));
      try {
        if (File.Exists(nuName)) {
          logger.Debug("file {0} already existed, so we remove it", nuName);
          File.Delete(nuName);
        }
        // only in straight mode there is no copy
        if (WorkingStyle == TASKWORKINGSTYLE.STRAIGHT) {
          File.Move(iwi.CurrentFile.FullName, nuName);
        } else {
          File.Copy(iwi.CurrentFile.FullName, nuName);
        }
        // when straight or copyinput we need to let currentfile point to the new file
        if (WorkingStyle != TASKWORKINGSTYLE.COPYOUTPUT) {
          // save current file location
          iwi.CurrentFile = new FileInfo(nuName);
        }
        success = true;
      } catch (Exception ex) {
        logger.ErrorException(string.Format("error while moving (renaming) file {0} to: {1}", iwi.CurrentFile.Name, nuName), ex);
      }

      return success;
    }

    private string RenamedString(ImageWI iwi)
    {
      // check for exif date...so we dont need to read exif data when it is not needed
      if (this.RenamePattern.Contains("{2}")) {
        return string.Format(this.RenamePattern, iwi.ProcessPosition, iwi.FileDate, iwi.ExifDate) + iwi.OriginalFile.Extension;
      } else {
        return string.Format(this.RenamePattern, iwi.ProcessPosition, iwi.FileDate, 0) + iwi.OriginalFile.Extension;
      }
    }
  }
}