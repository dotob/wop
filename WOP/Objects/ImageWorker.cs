using System.Diagnostics;
using System.Drawing;
using System.IO;
using Fireball.Drawing;

namespace WOP.Objects
{
    /// <summary>
    /// this is for editing pictures
    /// </summary>
    public class ImageWorker
    {
        public static void ShrinkImageIM(FileInfo fileIn, FileInfo fileOut, Size nuSize)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "convert.exe";
            psi.Arguments = string.Format("\"{0}\" -resize {1}x{2} \"{3}\"", fileIn.FullName, nuSize.Width,
                                          nuSize.Height, fileOut.FullName, nuSize.Width*2, nuSize.Height*2);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            var imp = new Process();
            imp.StartInfo = psi;
            imp.Start();
            imp.WaitForExit();
            string s = imp.StandardError.ReadToEnd();
            s = imp.StandardOutput.ReadToEnd();
        }

        public static void ShrinkImageFI(FileInfo fileIn, FileInfo fileOut, Size nuSize)
        {
            var fi = new FreeImage(fileIn.FullName);
            fi.Rescale(nuSize.Width, nuSize.Height);
            fi.Save(fileOut.FullName);
        }
    }
}