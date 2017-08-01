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
                watcher.Log(sourceFileName, "Copied");
            }
            catch (Exception e)
            {
                //BUG: необходимо дождаться завершения копирования большего файла
                watcher.Log(sourceFileName, "Copy error:" + Environment.NewLine + e.ToString());
                return;
            }

            // convert
        }
    }
}
