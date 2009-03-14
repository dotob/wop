using System;
using System.IO;
using System.Windows.Forms;

namespace WOP.Util
{
  public static class Utils
  {
    public static string NameWithoutExtension(this FileInfo fi)
    {
      return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
    }

    public static string AugmentFilename(this FileInfo fi, string augmentWith)
    {
      return Path.Combine(fi.DirectoryName, string.Format("{0}{1}{2}", fi.NameWithoutExtension(), augmentWith, fi.Extension));
    }

    public static FileInfo GetFileFromDialog(string initialDir)
    {
      FileInfo fi = null;
      var ofd = new OpenFileDialog();
      ofd.InitialDirectory = initialDir;
      if (ofd.ShowDialog() == DialogResult.OK) {
        fi = new FileInfo(ofd.FileName);
      }
      return fi;
    }

    public static DirectoryInfo GetDirFromDialog(string initialDir)
    {
      DirectoryInfo di = new DirectoryInfo(initialDir);
      var ofd = new FolderBrowserDialog();
      ofd.SelectedPath = initialDir;
      ofd.ShowNewFolderButton = true;
      if (ofd.ShowDialog() == DialogResult.OK) {
        di = new DirectoryInfo(ofd.SelectedPath);
      }
      return di;
    }

    public static BogenMass ConvertToBogenMass(double inDegrees)
    {
      BogenMass bm = new BogenMass();
      bm.Grad = (byte)Math.Truncate(inDegrees);
      bm.Minuten = (byte)((inDegrees - bm.Grad) * 60);
      bm.Sekunden = ((inDegrees - bm.Grad) * 60 - bm.Minuten) * 60;
      bm.Plus = inDegrees >= 0;
      return bm;
    }

    public static void garanteeDirExists(string directory)
    {
      if (!Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }
    }
  }
}