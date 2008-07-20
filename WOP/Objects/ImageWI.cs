using System;
using System.IO;

namespace WOP.Objects {
    public class ImageWI : IWorkItem {
        public ImageWI(FileInfo fi)
        {
            CreationTime = DateTime.Now;
            OriginalFile = fi;
            CurrentFile = fi;
        }

        #region IWorkItem Members

        public int ProcessPosition { get; set; }
        public int SortedPosition { get; set; }
        public FileInfo CurrentFile { get; set; }

        public FileInfo OriginalFile { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime FinishedWork { get; set; }

        #endregion
    }
}