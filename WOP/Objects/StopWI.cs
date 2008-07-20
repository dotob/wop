using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WOP.Objects
{
    /// <summary>
    /// this is a marker item so the job can tell the tasks that its end is reached
    /// </summary>
    public class StopWI:IWorkItem
    {
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
    }
}
