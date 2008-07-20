using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WOP.Objects
{
    public interface IWorkItem
    {
        int ProcessPosition { get; set; }
        int SortedPosition { get; set; }
        FileInfo CurrentFile { get; set; }
        FileInfo OriginalFile { get; set; }
        DateTime CreationTime { get; set; }
        DateTime FinishedWork { get; set; }
    }
}
