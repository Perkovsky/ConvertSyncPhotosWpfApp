using System.IO;
using System.Reflection;

namespace ConvertSyncPhotosWpfApp
{
    public static class GeneralMethods
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
        //public static void Convert(Watcher watcher, string sourceFileName, string destFileName)
        //{
        //    // copy to convert directory
        //    try
        //    {
        //        File.Copy(sourceFileName, destFileName, true);
        //        watcher.Log(sourceFileName, "Copied");
        //    }
        //    catch (Exception e)
        //    {
        //        watcher.Log(sourceFileName, "Copy error:" + Environment.NewLine + e.ToString());
        //        return;
        //    }
        //
        //    // convert
        //}
    }
}
