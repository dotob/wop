using System;
using System.IO;
using System.Windows.Controls;
using FreeImageAPI;
using WOP.Objects;
using WOP.TasksUI;
using WOP.Util;

namespace WOP.Tasks
{
  public class ImageRotateTask : SkeletonTask
  {
    private string nameExtension = "_rotated";
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
      t.WorkingStyle = this.WorkingStyle;
      return t;
    }

    public override bool Process(ImageWI iwi)
    {
      // what to do?
      switch (this.WorkingStyle) {
        case TASKWORKINGSTYLE.STRAIGHT:
          // destroy fib (will be reloaded by next task)
          iwi.CleanUp();
          // save to temp file
          FileInfo tmpFI;
          if (iwi.ImageHandleWeak != null) {
            // when we hav a image handle save that to disk
            tmpFI = new FileInfo(Path.Combine(iwi.CurrentFile.DirectoryName ?? "./", DateTime.Now.Millisecond.ToString()));
            ImageWorker.SaveJPGImageHandle(iwi.ImageHandle, tmpFI);
          }
          else {
            // if we dont have a image handle use currentfileinfo
            tmpFI = iwi.CurrentFile;
          }
          // transform temp file
          ImageWorker.AutoRotateImageFI(tmpFI, this.nameExtension);
          // load into fib
          FIBITMAP? fibitmap = ImageWorker.GetJPGImageHandle(tmpFI);
          if (fibitmap != null) {
            iwi.ImageHandle = (FIBITMAP)fibitmap;
          }
          else {
            // tell anyone?
          }
          // remove tempfile
          tmpFI.Delete();
          break;
        case TASKWORKINGSTYLE.COPYOUTPUT:
          // leave fib alone
          // save current version, if necessary
          if (iwi.ImageHandleWeak != null) {
            ImageWorker.SaveJPGImageHandle(iwi.ImageHandle, iwi.CurrentFile);
          }
          // transform current version to new name
          ImageWorker.AutoRotateImageFI(iwi.CurrentFile, this.nameExtension);
          break;
        case TASKWORKINGSTYLE.COPYINPUT:
          // destroy fib (will be reloaded by next task)
          // save current file
          // transform file to new name
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      ImageWorker.AutoRotateImageFI(iwi.CurrentFile, string.Empty);
      return true;
    }
  }
}