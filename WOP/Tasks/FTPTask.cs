﻿using System;
using FTPLib;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks
{
  public class FTPTask : SkeletonTask
  {
    private readonly FTP ftplib = new FTP();
    private bool inited;

    public FTPTask()
    {
      this.Name = "FTP";
      this.UI = new FTPTaskUI();
      this.UI.DataContext = this;
    }

    [SettingProperty]
    public string Server { get; set; }
    [SettingProperty]
    public string ServerDirectory { get; set; }
    [SettingProperty]
    public string UserName { get; set; }
    [SettingProperty]
    public string Password { get; set; }

    public override ITask CloneNonDynamicStuff()
    {
      FTPTask t = new FTPTask();
      t.IsEnabled = this.IsEnabled;
      t.Server = this.Server;
      t.ServerDirectory = this.ServerDirectory;
      t.UserName = this.UserName;
      t.Password = this.Password;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      if (!this.inited) {
        this.ftplib.port = 21;
        this.ftplib.user = this.UserName;
        this.ftplib.pass = this.Password;
        this.ftplib.server = this.Server;
        this.inited = true;
      }

      this.ftplib.OpenUpload(iwi.CurrentFile.FullName, this.ServerDirectory + "/" + iwi.CurrentFile.Name);

      int perc;
      while (this.ftplib.DoUpload() > 0) {
        perc = (int)(((this.ftplib.BytesTotal) * 100) / this.ftplib.FileSize);
        Console.Write("\rUpload: {0}/{1} {2}%", this.ftplib.BytesTotal, this.ftplib.FileSize, perc);
        Console.Out.Flush();
      }
      Console.WriteLine();
      return true;
    }
  }
}