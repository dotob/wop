using System.Drawing;
using System.IO;
using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
    public class ImageShrinkTask : SkeletonTask {

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizePercent { get; set; }
        public bool PreserveOriginals { get; set; }
        public string NameExtension { get; set; }

        public ImageShrinkTask()
        {
            UI = new ImageShrinkTaskUI();
            UI.DataContext = this;
        }

        public override bool Process(ImageWI iwi)
        {
            var ftmp = iwi.CurrentFile;
            ImageWorker.ShrinkImageFI(iwi.CurrentFile, ftmp, new Size(400, 400));
            File.Delete(iwi.CurrentFile.FullName);
            File.Move(ftmp.FullName, iwi.CurrentFile.FullName);
            return true;
        }
    }
}