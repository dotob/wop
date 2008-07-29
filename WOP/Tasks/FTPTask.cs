using WOP.Objects;

namespace WOP.Tasks {
    public class FTPTask : SkeletonTask {
        public FTPTask(Job j) : base(j) {}

        public override bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}