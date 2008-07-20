using System.IO;

namespace WOP.Util {
    public static class Utils {
        public static string NameWithoutExtension(this FileInfo fi)
        {
            return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        }
    }
}