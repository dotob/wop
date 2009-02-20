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
      string nuName = Path.Combine(iwi.CurrentFile.DirectoryName ?? string.Empty, this.RenamedString(iwi));
      if (File.Exists(nuName)) {
        File.Delete(nuName);
      }
      File.Move(iwi.CurrentFile.FullName, nuName);
      // save current file location
      iwi.CurrentFile = new FileInfo(nuName);
      return true;
    }

    private string RenamedString(IWorkItem iwi)
    {
      return string.Format(this.RenamePattern, iwi.ProcessPosition, iwi.SortedPosition) + iwi.OriginalFile.Extension;
    }
  }
}