﻿using System;
using System.IO;

namespace WOP.Objects {
    public interface IWorkItem {
        int ProcessPosition { get; set; }
        int SortedPosition { get; set; }
        FileInfo CurrentFile { get; set; }
        FileInfo OriginalFile { get; set; }
        DateTime CreationTime { get; set; }
        DateTime FinishedWork { get; set; }
    }
}