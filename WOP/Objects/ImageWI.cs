using System;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects {
  public class ImageWI : IWorkItem {
    private DateTime? exifDate;
    private FIBITMAP? imageHandle;

    public ImageWI(FileInfo fi)
    {
      this.CreationTime = DateTime.Now;
      this.OriginalFile = fi;
      this.CurrentFile = fi;
      // get times
      this.FileDate = fi.CreationTime;
    }

    public DateTime FileDate { get; set; }

    public DateTime ExifDate
    {
      get
      {
        if (this.exifDate == null) {
          this.exifDate = ImageWorker.GetExifDate(this.ImageHandle);
        }
        return (DateTime) this.exifDate;
      }
      set { this.exifDate = value; }
    }

    #region IWorkItem Members

    public string Name
    {
      get { return this.OriginalFile.Name; }
    }

    public int ProcessPosition { get; set; }
    public int SortedPosition { get; set; }
    public FileInfo CurrentFile { get; set; }

    public FileInfo OriginalFile { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime FinishedWork { get; set; }

    public FIBITMAP ImageHandle
    {
      get
      {
        if (this.imageHandle == null) {
          this.imageHandle = ImageWorker.GetJPGImageHandle(this.CurrentFile);
        }
        return (FIBITMAP) this.imageHandle;
      }
      set { this.imageHandle = value; }
    }

    public void CleanUp()
    {
      if (this.imageHandle != null) {
        ImageWorker.CleanUpResources((FIBITMAP) this.imageHandle);
        this.imageHandle = FIBITMAP.Zero;
      }
    }

    #endregion

    public override string ToString()
    {
      if (this.OriginalFile != null && this.OriginalFile != null) {
        return string.Format("was: {0}  ; is: {1}", this.OriginalFile.FullName, this.CurrentFile.FullName);
      }
      return base.ToString();
    }
  }
}