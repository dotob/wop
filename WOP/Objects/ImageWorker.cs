using System.Diagnostics;
using System.Drawing;
using System.IO;
using FreeImageAPI;
using FreeImageAPI.Metadata;
using WOP.Util;

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
            ShrinkImageFI(fileIn, fileOut, nuSize, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_FILTER.FILTER_BOX, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD, false);
        }

        public static FIBITMAP ShrinkImageFI(FileInfo fileIn, FileInfo fileOut, Size nuSize, FREE_IMAGE_FORMAT saveFormat, FREE_IMAGE_FILTER filter, FREE_IMAGE_SAVE_FLAGS savequality, bool cleanup)
        {
            FIBITMAP dib = GetJPGImageHandle(fileIn);
            ShrinkImageFI(dib, nuSize, filter, fileOut, savequality);
            if (cleanup) {
                CleanUpResources(dib);
            }
            return dib;
        }

        private static void ShrinkImageFI(FIBITMAP dib, Size nuSize, FREE_IMAGE_FILTER filter, FileInfo fileOut, FREE_IMAGE_SAVE_FLAGS savequality)
        {
            FIBITMAP dibsmall = FreeImage.Rescale(dib, nuSize.Width, nuSize.Height, filter);
            FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dibsmall, fileOut.FullName, savequality);
            CleanUpResources(dibsmall);
        }

        /// <summary>
        /// this aquires and frees a freeimage bitmap handle
        /// see: http://sylvana.net/jpegcrop/exif_orientation.html
        /// </summary>
        /// <param name="fi">the file to rotate</param>
        /// <param name="extension">if you want a different filename, it can be extended</param>
        public static void AutoRotateImageFI(FileInfo fi, string extension)
        {
            FIBITMAP dib = GetJPGImageHandle(fi);
            string outname = Path.Combine(fi.DirectoryName, fi.NameWithoutExtension() + extension + fi.Extension);
            // Create a wrapper for all metadata the image contains
            var iMetadata = new ImageMetadata(dib);
            MetadataModel exifMain = iMetadata[FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN];
            if (exifMain != null) {
                MetadataTag orientationTag = exifMain.GetTag("Orientation");
                if (orientationTag != null) {
                    bool imageWasChanged = false;
                    var rotInfo = (ushort[]) orientationTag.Value;
                    ushort rotateme = rotInfo[0];
                    switch (rotateme) {
                        case 1:
                            // do nothing, alright
                            break;
                        case 2:
                            // flip vertical
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_V, true);
                            imageWasChanged = true;
                            break;
                        case 3:
                            // 180 clockwise
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_180, true);
                            imageWasChanged = true;
                            break;
                        case 4:
                            // flip horizontal
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_H, true);
                            imageWasChanged = true;
                            break;
                        case 5:
                            // flip horizontal, 90 clockwise
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_H, true);
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_90, true);
                            imageWasChanged = true;
                            break;
                        case 6:
                            // 90 clockwise
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_90, true);
                            imageWasChanged = true;
                            break;
                        case 7:
                            // flip vertical, 90 clockwise
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_FLIP_V, true);
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_90, true);
                            imageWasChanged = true;
                            break;
                        case 8:
                            // 270 clockwise
                            FreeImage.JPEGTransform(fi.FullName, outname, FREE_IMAGE_JPEG_OPERATION.FIJPEG_OP_ROTATE_270, true);
                            imageWasChanged = true;
                            break;
                    }
                    if (imageWasChanged) {
                        orientationTag.SetValue(new ushort[] {1});
                        //TODO: save file
                    }
                }
            }
            CleanUpResources(dib);
        }

        /// <summary>
        /// get the rotation info
        /// </summary>
        /// <param name="dib"></param>
        /// <returns></returns>
        public static ushort GetRotateInfo(FIBITMAP dib)
        {
            ushort rotateme = 1;
            var iMetadata = new ImageMetadata(dib);
            MetadataModel exifMain = iMetadata[FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN];
            if (exifMain != null) {
                MetadataTag orientationTag = exifMain.GetTag("Orientation");
                if (orientationTag != null) {
                    var rotInfo = (ushort[]) orientationTag.Value;
                    if (rotInfo != null && rotInfo.Length > 0) {
                        rotateme = rotInfo[0];
                    }
                }
            }
            return rotateme;
        }

        /// <summary>
        /// set the rotation info
        /// </summary>
        /// <remarks>FREEIMAGE (until 3.11) does not support writing of metadata!!!!!!!!!</remarks>
        /// <param name="dib"></param>
        /// <param name="rotInfo"></param>
        /// <returns>if the value was changed</returns>
        public static bool SetRotateInfo(FIBITMAP dib, ushort rotInfo)
        {
            bool changed = false;
            var iMetadata = new ImageMetadata(dib);
            MetadataModel exifMain = iMetadata[FREE_IMAGE_MDMODEL.FIMD_EXIF_MAIN];
            if (exifMain != null) {
                MetadataTag orientationTag = exifMain.GetTag("Orientation");
                if (orientationTag != null) {
                    var nuval = new ushort[] {rotInfo};
                    if (orientationTag.Value != nuval) {
                        orientationTag.SetValue(nuval);
                        changed = true;
                    }
                }
            }
            return changed;
        }

        public static FIBITMAP GetJPGImageHandle(FileInfo fileIn)
        {
            return FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, fileIn.FullName, FREE_IMAGE_LOAD_FLAGS.JPEG_FAST);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dib"></param>
        /// <param name="fileOut"></param>
        /// <param name="savequality"></param>
        /// <returns>true if success</returns>
        public static bool SaveJPGImageHandle(FIBITMAP dib, FileInfo fileOut, FREE_IMAGE_SAVE_FLAGS savequality)
        {
            return FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, fileOut.FullName, savequality);
        }        
        
        /// <summary>
        /// save image with good jpeg quality
        /// </summary>
        /// <param name="dib"></param>
        /// <param name="fileOut"></param>
        /// <returns></returns>
        public static bool SaveJPGImageHandle(FIBITMAP dib, FileInfo fileOut)
        {
            return FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, fileOut.FullName, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD);
        }

        public static void CleanUpResources(FIBITMAP dib)
        {
            // The bitmap was saved to disk but is still allocated in memory, so the handle has to be freed.
            if (!dib.IsNull) {
                FreeImage.Unload(dib);
            }
            // Make sure to set the handle to null so that it is clear that the handle is not pointing to a bitmap.
            dib = 0;
        }
    }
}