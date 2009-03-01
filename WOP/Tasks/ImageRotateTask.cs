using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
  public class ImageRotateTask : SkeletonTask {
    public ImageRotateTask()
    {
      this.Name = "Drehen";
      this.UI = new ImageRotateTaskUI();
      this.UI.DataContext = this;
    }

    [SettingProperty]
    public bool PreserveOriginals { get; set; }
    [SettingProperty]
    public string NameExtension { get; set; }

    public override ITask CloneNonDynamicStuff()
    {
      ImageRotateTask t = new ImageRotateTask();
      t.IsEnabled = this.IsEnabled;
      t.NameExtension = this.NameExtension;
      t.PreserveOriginals = this.PreserveOriginals;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      ImageWorker.AutoRotateImageFI(iwi.CurrentFile, string.Empty);
      return true;
    }
  }
}