using System.Drawing;
using System.IO;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks
{
  public class ImageShrinkTask : SkeletonTask
  {
    public ImageShrinkTask()
    {
      this.Name = "Verkleinern";
      this.UI = new ImageShrinkTaskUI();
      this.UI.DataContext = this;
    }

    public int SizeX { get; set; }
    public int SizeY { get; set; }
    public int SizePercent { get; set; }
    public bool PreserveOriginals { get; set; }
    public string NameExtension { get; set; }

    public override bool Process(ImageWI iwi)
    {
      var ftmp = new FileInfo(iwi.CurrentFile.AugmentFilename("_tmp_"));
      ImageWorker.ShrinkImageFI(iwi.CurrentFile, ftmp, new Size(400, 400));
      File.Delete(iwi.CurrentFile.FullName);
      File.Move(ftmp.FullName, iwi.CurrentFile.FullName);
      return true;
    }
  }
}