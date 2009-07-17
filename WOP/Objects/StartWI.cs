using System;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects {
  internal class StartWI : IWorkItem {
    #region IWorkItem Members

    public string Name
    {
      get { return "StartItem"; }
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

    public FIBITMAP? ImageHandleWeak
    {
      get { throw new NotImplementedException(); }
    }

    public void CleanUp()
    {
      // do notting
    }

    #endregion
  }
}