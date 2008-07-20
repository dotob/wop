using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WOP.Objects;

namespace WOP.Tasks
{
    public class ImageShrinkTask:SkeletonTask
    {
        public ImageShrinkTask(Job j) : base(j) { }
        public override bool Process(ImageWI iwi)
        {
            return true;
        }
    }
}
