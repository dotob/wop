using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks {
  [JsonObject(MemberSerialization.OptIn)]
  public class ImageShrinkTask : SkeletonTask {
    private int sizePercent;
    private int sizeX;
    private int sizeY;
    private UserControl ui;

    public ImageShrinkTask()
    {
      this.Name = "Verkleinern";
      this.AbsoluteSizing = true;
      this.SizePercent = 50;
    }

    public override UserControl UI
    {
      get
      {
        this.ui = new ImageShrinkTaskUI();
        this.ui.DataContext = this;
        return this.ui;
      }
      set { this.ui = value; }
    }

    [JsonProperty]
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

    [JsonProperty]
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

    [JsonProperty]
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

    [JsonProperty]
    public bool AbsoluteSizing { get; set; }
    [JsonProperty]
    public bool PreserveOriginals { get; set; }
    [JsonProperty]
    public string NameExtension { get; set; }

    public override ITask CloneNonDynamicStuff()
    {
      ImageShrinkTask t = new ImageShrinkTask();
      t.IsEnabled = this.IsEnabled;
      t.AbsoluteSizing = this.AbsoluteSizing;
      t.NameExtension = this.NameExtension;
      t.PreserveOriginals = this.PreserveOriginals;
      t.SizePercent = this.SizePercent;
      t.SizeX = this.SizeX;
      t.SizeY = this.SizeY;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      try {
        FileInfo ftmp = new FileInfo(iwi.CurrentFile.AugmentFilename("_tmp_"));
        ImageWorker.ShrinkImageFI(iwi.ImageHandle, ftmp, this.calcNewSize(iwi));
        File.Delete(iwi.CurrentFile.FullName);
        File.Move(ftmp.FullName, iwi.CurrentFile.FullName);
      } catch (Exception ex) {
        logger.ErrorException(string.Format("error while shrinking file {0}", iwi.CurrentFile.Name), ex);
      }
      return true;
    }

    private Size calcNewSize(ImageWI iwi)
    {
      Size oldSize = ImageWorker.GetCurrentSize(iwi);
      logger.Debug("oldsize of image {0}:{1}", iwi.Name, oldSize);
      float ratio = oldSize.Width*1f/oldSize.Height;
      Size newSize;
      if (this.AbsoluteSizing) {
        newSize = new Size(this.SizeX, (int) (this.SizeX/ratio));
        logger.Debug("newsize (absolut) of image {0}:{1}", iwi.Name, newSize);
      } else {
        // suppose relativesizing
        newSize = new Size((int) (oldSize.Width*(this.SizePercent/100f)), (int) (oldSize.Height*(this.SizePercent/100f)));
        logger.Debug("newsize (percentage) of image {0}:{1}", iwi.Name, newSize);
      }
      return newSize;
    }
  }
}