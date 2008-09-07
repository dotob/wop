using System;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects {
    public class ImageWI : IWorkItem {
        public ImageWI(FileInfo fi)
        {
            CreationTime = DateTime.Now;
            OriginalFile = fi;
            CurrentFile = fi;
            // get times
            FileDate = fi.CreationTime;
            ExifDate = fi.CreationTime;
        }

        public DateTime FileDate { get; set; }
        public DateTime ExifDate { get; set; }

        #region IWorkItem Members

        public int ProcessPosition { get; set; }
        public int SortedPosition { get; set; }
        public FileInfo CurrentFile { get; set; }

        public FileInfo OriginalFile { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime FinishedWork { get; set; }

        public FIBITMAP ImageHandle{ get; set; }
        public void CleanUp()
        {
            
        }

        #endregion

        public override string ToString()
        {
            if (OriginalFile != null && OriginalFile != null) {
                return string.Format("was: {0}  ; is: {1}", OriginalFile.FullName, CurrentFile.FullName);
            }
            return base.ToString();
        }
    }
}