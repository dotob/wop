using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WOP.Objects;
using WOP.Util;

namespace WOP.Tasks
{
    public class FileRenamerTask:SkeletonTask
    {
        public string RenamePattern { get; set; }

        public override bool Process(ImageWI iwi)
        {
            string nuName = Path.Combine(iwi.CurrentFile.DirectoryName ?? string.Empty, RenamedString(iwi));
            File.Move(iwi.CurrentFile.FullName, nuName);
            // save current file location
            iwi.CurrentFile = new FileInfo(nuName);
            return true;
        }

        private string RenamedString(IWorkItem iwi)
        {
            return string.Format(RenamePattern, iwi.ProcessPosition, iwi.SortedPosition);
        }
    }
}
