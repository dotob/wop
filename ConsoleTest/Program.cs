using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WOP.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
//            Job j = Job.CreateTestJob();
//            j.Start();
 
            Thread t = new Thread(FileGatherTask.Test);
            t.Start();
        }
    }
}
