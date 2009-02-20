using System;
using System.Drawing;
using System.IO;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks
{
  public class ImageShrinkTask : SkeletonTask
  {
    private int sizePercent;
    private int sizeX;
    private int sizeY;

    public ImageShrinkTask()
    {
      this.Name = "Verkleinern";
      this.AbsoluteSizing = true;
      this.SizePercent = 50;
      this.UI = new ImageShrinkTaskUI();
      this.UI.DataContext = this;
    }

    public int SizeX
    {
      get { return this.sizeX; }
      set
      {
        if (this.sizeX == value) {
          return;
        }
        this.sizeX = value;
        this.RaisePropertyChangedEvent("SizeX");
      }
    }

    public int SizeY
    {
      get { return this.sizeY; }
      set
      {
        if (this.sizeY == value) {
          return;
        }
        this.sizeY = value;
        this.RaisePropertyChangedEvent("SizeY");
      }
    }

    public int SizePercent
    {
      get { return this.sizePercent; }
      set
      {
        if (this.sizePercent == value) {
          return;
        }
        this.sizePercent = value;
        this.RaisePropertyChangedEvent("SizePercent");
      }
    }

    public bool AbsoluteSizing { get; set; }
    public bool PreserveOriginals { get; set; }
    public string NameExtension { get; set; }

    public override bool Process(ImageWI iwi)
    {
      try {
        FileInfo ftmp = new FileInfo(iwi.CurrentFile.AugmentFilename("_tmp_"));
        ImageWorker.ShrinkImageFI(iwi.ImageHandle, ftmp, this.calcNewSize(iwi));
        File.Delete(iwi.CurrentFile.FullName);
        File.Move(ftmp.FullName, iwi.CurrentFile.FullName);
      }catch(Exception ex)
      {
        logger.ErrorException(string.Format("error while shrinking file {0}", iwi.CurrentFile.Name), ex);
      }
      return true;
    }

    private Size calcNewSize(ImageWI iwi)
    {
      Size oldSize = ImageWorker.GetCurrentSize(iwi);
      logger.Debug("oldsize of image {0}:{2}", iwi.OriginalFile.Name, oldSize);
      double ratio = (double)oldSize.Width / oldSize.Height;
      Size newSize;
      if (this.AbsoluteSizing) {
        newSize = new Size(this.SizeX, (int)(this.SizeX / ratio));
        logger.Debug("newsize (absolut) of image {0}:{2}", iwi.OriginalFile.Name, newSize);
      }
      else {
        // suppose relativesizing
        newSize = new Size((int)(oldSize.Width * (this.SizePercent / 100f)), (int)(oldSize.Height * (this.SizePercent / 100f)));
        logger.Debug("newsize (percentage) of image {0}:{2}", iwi.OriginalFile.Name, newSize);
      }
      return newSize;
    }
  }
}