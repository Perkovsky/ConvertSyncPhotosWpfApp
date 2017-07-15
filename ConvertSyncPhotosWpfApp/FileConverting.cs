using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertSyncPhotosWpfApp
{
    public static class FileConverting
    {
        public static string GetCurrentDirectory()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = Path.GetDirectoryName(path);
            return path;
        }

        /// <summary>
        /// This method copy to convert directory, resize and compress file-photo 
        /// </summary>
        public static void Convert(Watcher watcher, string sourceFileName, string destFileName)
        {
            // copy to convert directory
            try
            {
                File.Copy(sourceFileName, destFileName, true);
                watcher.Log(string.Format("{0} -> {1}", Path.GetFileName(sourceFileName), "Copied"));
            }
            catch (Exception e)
            {
                watcher.Log(string.Format("{0} -> {1}", Path.GetFileName(sourceFileName), "Copy error:"));
                watcher.Log(e.ToString());
                return;
            }

            // convert
        }
    }
}
