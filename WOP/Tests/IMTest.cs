using System;
using System.Drawing;
using System.IO;
using Fireball.Drawing;
using NUnit.Framework;
using WOP.Objects;
using WOP.Tasks;

namespace WOP.Tests
{
    [TestFixture]
    public class IMTest
    {
        [Test]
        public void TestIMSpeed()
        {
            DateTime start = DateTime.Now;
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "*jpg"))
            {
                var fin = new FileInfo(s);
                var fout = new FileInfo(Path.Combine(fin.DirectoryName, fin.Name + "_small" + fin.Extension));
                ImageWorker.ShrinkImageFI(fin, fout, new Size(400, 400));
            }
            TimeSpan ts = DateTime.Now.Subtract(start);
            string d = Directory.GetCurrentDirectory();
            Console.WriteLine(ts);
        }

        [Test]
        public void TagTest()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "*jpg")) {
                FreeImage fifi = new FreeImage(s);
            }
        }

        [Test]
        public void FITest()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\IM\pix", "*jpg")) {
                FileInfo fi = new FileInfo(s);
                fi.Name.ToString();
                fi.Extension.ToString();
                fi.FullName.ToString();
                fi.DirectoryName.ToString();
            }
        }

        [Test]
        public void JobTest()
        {
            Job j = Job.CreateTestJob();
            j.Start();
        }


    }
}