using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks
{
    public class FileRenamerTask:SkeletonTask
    {
        public FileRenamerTask(Job j) : base(j) { }

        public override bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}
