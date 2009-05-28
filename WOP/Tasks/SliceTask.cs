using System.Drawing;
using System.IO;
using System.Windows.Controls;
using FreeImageAPI;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks {
  public class SliceTask : SkeletonTask {
    private bool inited;
    private UserControl ui;

    public SliceTask()
    {
      this.Name = "Zerschneiden";
    }

    public override UserControl UI
    {
      get
      {
        this.ui = new SliceTaskUI();
        this.ui.DataContext = this;
        return this.ui;
      }
      set { this.ui = value; }
    }

    public int XSliceCount { get; set; }
    public int YSliceCount { get; set; }

    public override ITask CloneNonDynamicStuff()
    {
      SliceTask t = new SliceTask();
      t.IsEnabled = this.IsEnabled;
      t.XSliceCount = this.XSliceCount;
      t.YSliceCount = this.YSliceCount;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      Size s = ImageWorker.GetCurrentSize(iwi);
      Size tileSize = new Size();
      tileSize.Width = s.Width/this.XSliceCount;
      tileSize.Height = s.Height/this.YSliceCount;
      int i = 0;
      for (int x = 0; x < this.XSliceCount; x++) {
        for (int y = 0; y < this.YSliceCount; y++) {
          int left = x*tileSize.Width;
          int top = y*tileSize.Height;
          FIBITMAP aTile = FreeImage.Copy(iwi.ImageHandle, left, top, left + tileSize.Width, top + tileSize.Height);
          ImageWorker.SaveJPGImageHandle(aTile, new FileInfo(iwi.CurrentFile.AugmentFilename(string.Format("_tile_{0:000}", i))));
          i++;
        }
      }
      return true;
    }
  }
}