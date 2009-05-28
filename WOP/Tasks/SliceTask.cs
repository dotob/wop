using System;
using System.Windows.Controls;
using FTPLib;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks
{
  public class SliceTask : SkeletonTask
  {
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
      return true;
    }
  }
}