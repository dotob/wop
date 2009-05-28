﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using FreeImageAPI;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks {
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
    public bool HandoverOriginal { get; set; }

    public override ITask CloneNonDynamicStuff()
    {
      ImageShrinkTask t = new ImageShrinkTask();
      t.IsEnabled = this.IsEnabled;
      t.AbsoluteSizing = this.AbsoluteSizing;
      t.NameExtension = this.NameExtension;
      t.PreserveOriginals = this.PreserveOriginals;
      t.SizePercent = this.SizePercent;
      t.HandoverOriginal = this.HandoverOriginal;
      t.SizeX = this.SizeX;
      t.SizeY = this.SizeY;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      try {
        if (this.PreserveOriginals) {
          FileInfo ftmp = new FileInfo(iwi.CurrentFile.AugmentFilename(this.NameExtension));
          // save shrinked version of image to file
          FIBITMAP shrinkedDib = ImageWorker.GetShrinkedDIB(iwi.ImageHandle, this.calcNewSize(iwi));
          ImageWorker.SaveJPGImageHandle(shrinkedDib, ftmp);
          // now we have the original and the shrinked version on disk
          // check which version should be handed over to the next task
          if (!this.HandoverOriginal) {
            // after this the smaller image is the new current
            ImageWorker.CleanUpResources(iwi.ImageHandle);
            // set it the new current
            iwi.ImageHandle = shrinkedDib;
            iwi.CurrentFile = ftmp;
          } else {
            // handle for shrinked version is not needed anymore
            ImageWorker.CleanUpResources(shrinkedDib);
          }
        } else {
          // create shrinked image in memory
          FIBITMAP shrinkedDib = ImageWorker.GetShrinkedDIB(iwi.ImageHandle, this.calcNewSize(iwi));
          // cleanup old current handle
          ImageWorker.CleanUpResources(iwi.ImageHandle);
          // set shrinked version the new current
          iwi.ImageHandle = shrinkedDib;
          // delete original file
          File.Delete(iwi.CurrentFile.FullName);
          // save shrinked image to disk
          ImageWorker.SaveJPGImageHandle(iwi.ImageHandle, iwi.CurrentFile);
        }
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