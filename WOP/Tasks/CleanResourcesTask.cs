using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks
{
  /// <summary>
  /// remove memory used by the resources
  /// </summary>
  public class CleanResourcesTask:SkeletonTask
  {
    public CleanResourcesTask()
    {
      this.Name = "Speicher aufräumen";
    }

    public override UserControl UI
    {
      get
      {
        return null;
      }
      set {  }
    }

    /// <summary>
    /// this task can only bee at first position
    /// </summary>
    public new TASKPOS Position
    {
      get { return base.Position; }
      set
      {
        if (value != TASKPOS.LAST){
          throw new ArgumentException("Der CleanResourcesTask kann nur an letzter Stelle eines Jobs kommen.", "Position");
        }
        base.Position = value;
      }
    }

    public override System.Windows.Visibility UIVisibility
    {
      get
      {
        return Visibility.Collapsed;
      }
    }

    public override ITask CloneNonDynamicStuff()
    {
      CleanResourcesTask t = new CleanResourcesTask();
      t.IsEnabled = this.IsEnabled;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      logger.Info("task {0} start processing: {1}", this.Name, iwi);
      iwi.CleanUp();
      logger.Info("task {0} cleaned memory for: {1}", this.Name, iwi);
      return true;
    }
  }
}
