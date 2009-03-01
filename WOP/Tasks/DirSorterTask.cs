using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks
{
  public class DirSorterTask:SkeletonTask
  {
    public DirSorterTask()
    {
      this.Name = "In Ordner verteilen";
      this.UI = new DirSorterTaskUI();
      this.UI.DataContext = this;
    }

    public override ITask CloneNonDynamicStuff()
    {
      DirSorterTask t = new DirSorterTask();
      t.IsEnabled = this.IsEnabled;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      return true;
    }
  }
}
