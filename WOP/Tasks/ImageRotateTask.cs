﻿using System.Windows.Controls;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
  public class ImageRotateTask : SkeletonTask {
    private UserControl ui;

    public ImageRotateTask()
    {
      this.Name = "Drehen";
    }

    public override UserControl UI
    {
      get
      {
        this.ui = new ImageRotateTaskUI();
        this.ui.DataContext = this;
        return this.ui;
      }
      set { this.ui = value; }
    }

    public bool PreserveOriginals { get; set; }

    private string nameExtension = "_rotated";

    public string NameExtension
    {
      get { return this.nameExtension; }
      set { this.nameExtension = value; }
    }

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