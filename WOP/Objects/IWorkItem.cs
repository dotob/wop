using System;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects
{
  public interface IWorkItem
  {
    string Name { get; }
    int ProcessPosition { get; set; }
    int SortedPosition { get; set; }
    FileInfo CurrentFile { get; set; }
    FileInfo OriginalFile { get; set; }
    DateTime CreationTime { get; set; }
    DateTime FinishedWork { get; set; }
    FIBITMAP ImageHandle { get; set; }
    FIBITMAP? ImageHandleWeak { get; }

    void CleanUp();
  }
}