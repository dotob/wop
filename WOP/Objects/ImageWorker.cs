using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            SaveJPGImageHandle(dib, fileOut, savequality);
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
            string outname = fi.AugmentFilename(extension);
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
                    var nuval = new[] {rotInfo};
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

        public static void WriteGPSDateIntoImage(FileInfo fin, FileInfo fout, WayPoint wp)
        {
            BogenMass lon = Utils.ConvertToBogenMass(wp.Longitude);
            BogenMass lat = Utils.ConvertToBogenMass(wp.Latitude);
            WriteLongLat(fin.FullName, fout != null ? fout.FullName : string.Empty, lat.Grad, lat.Minuten, lat.Sekunden, lon.Grad, lon.Minuten, lon.Sekunden, !lon.Plus, lat.Plus);
        }

        // also nice to know: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/GPS.html
        // got this example from: http://www.dotnetclan.com/gps_jpeg_exif.cs
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j) {
                if (encoders[j].MimeType == mimeType) {
                    return encoders[j];
                }
            }
            return null;
        }

        private static void WriteLongLat(string fileIn, string fileOut, byte latDeg, byte latMin, double latSec, byte lonDeg, byte lonMin, double lonSec, bool isWest, bool isNorth)
        {
            const int length = 25;
            Image img;
            byte secHelper;
            byte secRemains;
            PropertyItem[] PropertyItems;
            string FilenameTemp;
            Encoder Enc = Encoder.Transformation;
            var EncParms = new EncoderParameters(1);
            EncoderParameter EncParm;
            ImageCodecInfo CodecInfo = GetEncoderInfo("image/jpeg");

            // load the image to change 
            img = Image.FromFile(fileIn);
            PropertyItems = img.PropertyItems;
            int oldArrLength = PropertyItems.Length;
            var newProperties = new PropertyItem[oldArrLength];
            img.PropertyItems.CopyTo(newProperties, 0);
            newProperties[0].Id = 0x0002;
            newProperties[0].Type = 5; //5-R 4-L 3-S 
            newProperties[0].Len = length;
            newProperties[0].Value = new byte[length];
            try {
                for (int i = 0; i < length; i++) {
                    newProperties[0].Value[i] = 0;
                }
            } catch {}
            //PropertyItems[0].Value = Pic.GetPropertyItem(4).Value; // bDescription; 
            newProperties[0].Value[0] = latDeg;
            newProperties[0].Value[8] = latMin;
            secHelper = (byte) (latSec/2.56);
            secRemains = (byte) ((latSec - (secHelper*2.56))*100);
            newProperties[0].Value[16] = secRemains; // add to the sum below x_x_*17_+16 
            newProperties[0].Value[17] = secHelper; // multiply by 2.56 
            newProperties[0].Value[20] = 100;
            img.SetPropertyItem(newProperties[0]);
            newProperties[1].Id = 0x0004;
            newProperties[1].Type = 5; //5-R 4-L 3-S 
            newProperties[1].Len = length;
            newProperties[1].Value = new byte[length];
            try {
                for (int i = 0; i < length; i++) {
                    newProperties[1].Value[i] = 0;
                }
            } catch (Exception e) {
                Console.WriteLine("Error {0}", e);
            }
            newProperties[1].Value[0] = lonDeg;
            newProperties[1].Value[8] = lonMin;
            secHelper = (byte) (lonSec/2.56);
            secRemains = (byte) ((lonSec - (secHelper*2.56))*100);
            newProperties[1].Value[16] = secRemains;
            // add to the sum bellow x_x_*17_+16 
            newProperties[1].Value[17] = secHelper;
            // multiply by 2.56 
            newProperties[1].Value[20] = 100;
            // multiply by 2.56 

            //PropertyItem current = Pic.GetPropertyItem(2); 
            img.SetPropertyItem(newProperties[1]);
            //GPS Version 
            newProperties[0].Id = 0x0000;
            newProperties[0].Type = 1;
            newProperties[0].Len = 4;
            newProperties[0].Value[0] = 2;
            newProperties[0].Value[1] = 2;
            newProperties[0].Value[2] = 0;
            newProperties[0].Value[3] = 0;
            img.SetPropertyItem(newProperties[0]);

            //GPS Lat REF 
            newProperties[0].Id = 0x0001;
            newProperties[0].Type = 2;
            newProperties[0].Len = 2;
            if (isNorth) {
                newProperties[0].Value[0] = 78; //ASCII for N
            } else {
                newProperties[0].Value[0] = 83; //ASCII for S
            }

            newProperties[0].Value[1] = 0;
            img.SetPropertyItem(newProperties[0]);


            //GPS Lon REF 
            newProperties[0].Id = 0x0003;
            newProperties[0].Type = 2; //5-R 4-L 3-S 
            newProperties[0].Len = 2;
            if (isWest == false) {
                newProperties[0].Value[0] = 69; //ASCII for E
            } else {
                newProperties[0].Value[0] = 87; //ASCII for W
            }
            newProperties[0].Value[1] = 0;
            img.SetPropertyItem(newProperties[0]);

            // we cannot store in the same image, so use a temporary image instead 
            FilenameTemp = fileIn + ".temp";
            // for lossless rewriting must rotate the image by 90 degrees! 
            EncParm = new EncoderParameter(Enc, (long) EncoderValue.TransformRotate90);
            EncParms.Param[0] = EncParm;
            // now write the rotated image with new description 
            img.Save(FilenameTemp, CodecInfo, EncParms);
            // for computers with low memory and large pictures: release memory now 
            img.Dispose();
            img = null;
            GC.Collect();
            // delete the original file, will be replaced later 
            if (string.IsNullOrEmpty(fileOut)) {
                File.Delete(fileIn);
            }
            // now must rotate back the written picture 
            img = Image.FromFile(FilenameTemp);
            EncParm = new EncoderParameter(Enc, (long) EncoderValue.TransformRotate270);
            EncParms.Param[0] = EncParm;
            if (string.IsNullOrEmpty(fileOut)) {
                img.Save(fileIn, CodecInfo, EncParms);
            } else {
                img.Save(fileOut, CodecInfo, EncParms);
            }
            // release memory now 
            img.Dispose();
            img = null;
            GC.Collect();
            // delete the temporary picture 
            File.Delete(FilenameTemp);
        }
    }
}