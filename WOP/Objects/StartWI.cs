using System;
using System.IO;
using FreeImageAPI;
using WOP.Objects;

namespace WOP.Objects
{
  internal class StartWI : IWorkItem
  {
    public string Name
    {
      get { return "StartItem"; }
    }

    public int ProcessPosition
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public int SortedPosition
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public FileInfo CurrentFile
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public FileInfo OriginalFile
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public DateTime CreationTime
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public DateTime FinishedWork
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public FIBITMAP ImageHandle
    {
      get { throw new System.NotImplementedException(); }
      set { throw new System.NotImplementedException(); }
    }

    public void CleanUp()
    {
      throw new System.NotImplementedException();
    }
  }
}