using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using WOP.Objects;
using WOP.Tasks;

namespace WOP.Tests {
    [TestFixture]
    public class IMTest {
        [Test]
        public void FITest()
        {

        }

        [Test]
        public void JobTest()
        {
            Job j = Job.CreateTestJob();
            j.Start();
        }

        [Test]
        public void TagTest()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "*jpg")) {
                //FreeImage fifi = new FreeImage(s);
            }
        }

        [Test]
        public void TestIMSpeed()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "*small*")) {
                File.Delete(s);
            }
            DateTime start = DateTime.Now;
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "test*jpg")) {
                var fin = new FileInfo(s);
                var fout = new FileInfo(Path.Combine(fin.DirectoryName, "small_"+fin.Name + fin.Extension));
                ImageWorker.ShrinkImageFI(fin, fout, new Size(400, 400));
            }
            TimeSpan ts = DateTime.Now.Subtract(start);
            string d = Directory.GetCurrentDirectory();

            Assert.AreEqual(0, 1, "duration: " + ts.Seconds);
        }
    }
}