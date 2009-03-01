using System;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects {
  /// <summary>
  /// this is a marker item so the job can tell the tasks that its end is reached
  /// </summary>
  public class StopWI : IWorkItem {
    #region IWorkItem Members

    public string Name
    {
      get { return "StopItem"; }
    }

    public int ProcessPosition
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public int SortedPosition
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public FileInfo CurrentFile
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public FileInfo OriginalFile
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public DateTime CreationTime
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public DateTime FinishedWork
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public FIBITMAP ImageHandle
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public void CleanUp()
    {
      // do notting
    }

    #endregion

    public override string ToString()
    {
      return "stop me";
    }
  }
}