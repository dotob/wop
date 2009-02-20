using System;
using System.IO;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks
{
  public class FileRenamerTask : SkeletonTask
  {

    public FileRenamerTask()
    {
      this.Name = "Umbenennen";
      this.UI = new FileRenamerTaskUI();
      this.UI.DataContext = this;
    }

    public string RenamePattern { get; set; }

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
        File.Move(iwi.CurrentFile.FullName, nuName);
        // save current file location
        iwi.CurrentFile = new FileInfo(nuName);
        success = true;
      }
      catch (Exception ex) {
        logger.ErrorException(string.Format("while moving (renaming) file {0} to: {1}", iwi.CurrentFile.Name, nuName), ex);
      }

      return success;
    }

    private string RenamedString(IWorkItem iwi)
    {
      return string.Format(this.RenamePattern, iwi.ProcessPosition, iwi.SortedPosition) + iwi.OriginalFile.Extension;
    }
  }
}