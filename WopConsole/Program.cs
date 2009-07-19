using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FreeImageAPI;
using WOP.Objects;
using WOP.Tasks;
using WOP.Util;

namespace WopConsole
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      //SpeedTest();
      //RotateTest();
      MetaDataTest();
      //GetRotateInfo();
      //SetRotateInfo();
      //GPSTest();
      //BogenMass bm = Utils.ConvertToBogenMass(51.2345);
      //TestWriteGPS();
      //TestFormatting();
    }

    private static void MetaDataTest()
    {
      FileInfo fi = new FileInfo(@"..\..\..\testdata\pixrotate\IMG_4222.jpg");
      FIBITMAP dib = (FIBITMAP) ImageWorker.GetJPGImageHandle(fi);
      ImageWorker.SaveJPGImageHandle(dib, fi.AugmentFileInfo("_meta"));
    }


    private static void TestWriteGPS()
    {
      ImageWI iwi = new ImageWI(new FileInfo(@"..\..\..\testdata\gps\pic.jpg"));
      // tweak date to match the on in gpx file
      iwi.ExifDate = DateTime.Parse("01.09.2008");
      var wpl = GEOTagTask.initGPXFiles(new List<FileInfo> {new FileInfo(@"..\..\..\testdata\gps\testdata.gpx")});
      var wp = GEOTagTask.findWaypoint4Date(iwi.ExifDate, wpl);
      ImageWorker.WriteGPSDateIntoImage(iwi.CurrentFile, new FileInfo(iwi.CurrentFile.AugmentFilename("_withgps")), wp);
    }

    private static void GPSTest()
    {
      var wpl = GEOTagTask.initGPXFiles(new List<FileInfo> {new FileInfo(@"..\..\..\testdata\gps\testdata.gpx")});
    }

    private static void RotateTest()
    {
        ImageWorker.AutoRotateImageFI(new FileInfo(@"..\..\..\testdata\pixrotate\IMG_4222.jpg"), "_rot");
        ushort u = ImageWorker.GetRotateInfo((FIBITMAP)ImageWorker.GetJPGImageHandle(new FileInfo(@"..\..\..\testdata\pixrotate\IMG_4222_rot.jpg")));
    }

    private static void GetRotateInfo()
    {
      foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pixrotate", "*")) {
        FileInfo fi = new FileInfo(s);
        FIBITMAP? dib = ImageWorker.GetJPGImageHandle(fi);
        if (dib != null) {
          ushort info = ImageWorker.GetRotateInfo((FIBITMAP) dib);
        }
      }
    }

    private static void SetRotateInfo()
    {
      foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pixrotate", "*")) {
        FileInfo fi = new FileInfo(s);
        FIBITMAP? dib = ImageWorker.GetJPGImageHandle(fi);
        if (dib != null) {
          ImageWorker.SetRotateInfo((FIBITMAP)dib, 8);
          ImageWorker.SaveJPGImageHandle((FIBITMAP)dib, new FileInfo(fi.AugmentFilename("_rotinfo")));
        }
      }
    }

    private static void SpeedTest()
    {
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
      foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pix", "*small*")) {
        //File.Delete(s);
      }
      DateTime start = DateTime.Now;
      foreach (string s in Directory.GetFiles(@"..\..\..\testdata\pix", "test*jpg")) {
        var fin = new FileInfo(s);
        var fout = new FileInfo(Path.Combine(fin.DirectoryName, string.Format("t{0}_small_{1}_{2}.jpg", fin.NameWithoutExtension(), filter, quality)));
        ImageWorker.ShrinkImageFI(fin, fout, new Size(400, 400), FREE_IMAGE_FORMAT.FIF_JPEG, filter, quality, true);
        Console.WriteLine("{0} -> {1}", fin.Name, fout.Name);
      }
      TimeSpan ts = DateTime.Now.Subtract(start);
      string d = Directory.GetCurrentDirectory();
      return ts;
    }

    public static void TestFormatting()
    {
      Console.WriteLine(string.Format("{0:000}", 1));
      Console.WriteLine(string.Format("{0:000}", 10));
      Console.WriteLine(string.Format("{0:000}", 10000));
      Console.ReadKey();
    }
  }
}