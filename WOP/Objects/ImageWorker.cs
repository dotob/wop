using System.Diagnostics;
using System.Drawing;
using System.IO;
using FreeImageAPI;

namespace WOP.Objects {
    /// <summary>
    /// this is for editing pictures
    /// </summary>
    public class ImageWorker {
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
            ShrinkImageFI(fileIn, fileOut, nuSize, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_FILTER.FILTER_BOX, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD);
        }

        public static void ShrinkImageFI(FileInfo fileIn, FileInfo fileOut, Size nuSize, FREE_IMAGE_FORMAT saveFormat, FREE_IMAGE_FILTER filter, FREE_IMAGE_SAVE_FLAGS savequality)
        {
            FIBITMAP dib = 0;
            dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, fileIn.FullName, FREE_IMAGE_LOAD_FLAGS.JPEG_FAST);
            FIBITMAP dibsmall = FreeImage.Rescale(dib, nuSize.Width, nuSize.Height, filter);
            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dibsmall, fileOut.FullName, savequality);
            // The bitmap was saved to disk but is still allocated in memory, so the handle has to be freed.
            if (!dib.IsNull)
                FreeImage.Unload(dib);
            if (!dibsmall.IsNull)
                FreeImage.Unload(dibsmall);

            // Make sure to set the handle to null so that it is clear that the handle is not pointing to a bitmap.
            dib = 0;
            dibsmall = 0;
        }
    }
}