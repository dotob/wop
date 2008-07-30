using WOP.Objects;
using WOP.TasksUI;

namespace WOP.Tasks {
    public class GEOTagTask : SkeletonTask
    {
        public GEOTagTask()
        {
            UI = new GEOTagTaskUI();
            UI.DataContext = this;
        }

        public override bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}