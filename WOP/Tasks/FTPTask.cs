using System;
using FTPLib;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
    public class FTPTask : SkeletonTask {
        private readonly FTP ftplib = new FTP();
        private bool inited;

        public FTPTask()
        {
            UI = new FTPTaskUI();
            UI.DataContext = this;
        }

        public string Server { get; set; }
        public string ServerDirectory { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public override bool Process(ImageWI iwi)
        {
            if (!inited) {
                ftplib.port = 21;
                ftplib.user = UserName;
                ftplib.pass = Password;
                ftplib.server = Server;
                inited = true;
            }

            ftplib.OpenUpload(iwi.CurrentFile.FullName, ServerDirectory + "/" + iwi.CurrentFile.Name);

            int perc;
            while (ftplib.DoUpload() > 0) {
                perc = (int) (((ftplib.BytesTotal)*100)/ftplib.FileSize);
                Console.Write("\rUpload: {0}/{1} {2}%", ftplib.BytesTotal, ftplib.FileSize, perc);
                Console.Out.Flush();
            }
            Console.WriteLine();
            return true;
        }
    }
}