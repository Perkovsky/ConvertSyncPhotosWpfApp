using System;
using System.Threading.Tasks;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// This class is needed to copy file from source directory to destination directory
    /// </summary>
    public class Copier
    {
        /// <summary>
        /// The method waits for the file to be created
        /// </summary>
        /// <param name="fileName"></param>
        private void IsFileBusy(string fileName)
        {
            while (true)
            {
                try { using (FileStream SourceStream = File.Open(fileName, FileMode.Open)) break; }
                catch (FileNotFoundException) { break; }
                catch (IOException) { /* waiting... */ }
                catch (Exception) { break; }
            }
        }

        /// <summary>
        /// The Method copies source file asynchronously from source root directory to destination root directory 
        /// keeping structure of directories
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="sourceFullFileName">Source full file name</param>
        /// <param name="sourceDirectoryName">Source root directory full path</param>
        /// <param name="destDirectoryName">Destination root directory full path</param>
        /// <returns>Destination full file name or null if was error</returns>
        public async Task<string> CopyToAsync(Watcher watcher, string sourceFullFileName, string sourceDirectoryName, string destDirectoryName)
        {
            string sourceFileName = Path.GetFileName(sourceFullFileName);
            string sourceFullParentDirectory = Path.GetDirectoryName(sourceFullFileName);

            // fix if file copy in root parent directory
            string sourceParentDirectory = "";
            if (!Path.GetDirectoryName(sourceDirectoryName).Equals(sourceFullParentDirectory))
            {
                sourceParentDirectory = Path.GetFileName(sourceFullParentDirectory);
            }

            string destFullParentDirectory = string.Format(@"{0}{1}\", destDirectoryName, sourceParentDirectory);
            string destFileName = string.Format(@"{0}{1}", destFullParentDirectory, sourceFileName);

            // create destination directory if exists
            if (!Directory.Exists(destFullParentDirectory)) Directory.CreateDirectory(destFullParentDirectory);

            IsFileBusy(sourceFullFileName);
            try
            {
                using (FileStream SourceStream = File.Open(sourceFullFileName, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destFileName))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }
                watcher.Log(sourceFullFileName, "Copied");
                return destFileName;
            }
            //catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                watcher.Log(sourceFullFileName, "Copy error:" + Environment.NewLine + e.ToString());
                return null;
            }
        }
    }
}
