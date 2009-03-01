using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Newtonsoft.Json;
using WOP.Objects;

namespace WOP.Tasks
{
  /// <summary>
  /// remove memory used by the resources
  /// </summary>
  [JsonObject(MemberSerialization.OptIn)]
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
