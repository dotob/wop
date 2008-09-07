using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FreeImageAPI;
using WOP.Objects;
using WOP.Util;

namespace WopConsole {
    internal class Program {
        private static void Main(string[] args)
        {
            //SpeedTest();
            //RotateTest();
            SetRotateInfo();
        }

        private static void RotateTest()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pixrotate", "*"))
            {
                ImageWorker.AutoRotateImageFI(new FileInfo(s), "_rot");
            }
        }

        private static void SetRotateInfo()
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pixrotate", "*"))
            {
                FileInfo fi = new FileInfo(s);
                FIBITMAP dib = ImageWorker.GetJPGImageHandle(fi);
                ImageWorker.SetRotateInfo(dib, 8);
                ImageWorker.SaveJPGImageHandle(dib, new FileInfo(Path.Combine(fi.DirectoryName, fi.NameWithoutExtension() + "_rotinfo" + fi.Extension)));
            }
        }

        private static void SpeedTest() {
            var filters = new List<FREE_IMAGE_FILTER>();
            filters.Add(FREE_IMAGE_FILTER.FILTER_BICUBIC);
            filters.Add(FREE_IMAGE_FILTER.FILTER_BILINEAR);
            filters.Add(FREE_IMAGE_FILTER.FILTER_BOX);
            filters.Add(FREE_IMAGE_FILTER.FILTER_BSPLINE);
            //filters.Add(FREE_IMAGE_FILTER.FILTER_CATMULLROM);
            //filters.Add(FREE_IMAGE_FILTER.FILTER_LANCZOS3);

            var qualis = new List<FREE_IMAGE_SAVE_FLAGS>();
            qualis.Add(FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYAVERAGE);
            //qualis.Add(FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYBAD);
            qualis.Add(FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD);
            qualis.Add(FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL);
            qualis.Add(FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB);

            var ergs = new List<string>();
            foreach (FREE_IMAGE_SAVE_FLAGS quali in qualis) {
                foreach (FREE_IMAGE_FILTER filter in filters) {
                    TimeSpan ts = DoTest(filter, quali);
                    ergs.Add(string.Format("{0} | {1} | {2}", ts, quali, filter));
                }
            }
            File.WriteAllLines(@"..\..\..\testdata\pix\results.txt", ergs.ToArray());
        }

        private static TimeSpan DoTest(FREE_IMAGE_FILTER filter, FREE_IMAGE_SAVE_FLAGS quality)
        {
            foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pix", "*small*"))
            {
                //File.Delete(s);
            }
            DateTime start = DateTime.Now;
            foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pix", "test*jpg"))
            {
                var fin = new FileInfo(s);
                var fout = new FileInfo(Path.Combine(fin.DirectoryName, string.Format("t{0}_small_{1}_{2}.jpg", fin.NameWithoutExtension(), filter, quality)));
                ImageWorker.ShrinkImageFI(fin, fout, new Size(400, 400), FREE_IMAGE_FORMAT.FIF_JPEG, filter, quality, true);
                Console.WriteLine("{0} -> {1}", fin.Name, fout.Name);
            }
            TimeSpan ts = DateTime.Now.Subtract(start);
            string d = Directory.GetCurrentDirectory();
            return ts;
        }
    }
}