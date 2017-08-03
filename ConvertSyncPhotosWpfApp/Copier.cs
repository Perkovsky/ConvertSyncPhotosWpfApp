using System;
using System.Threading.Tasks;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
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

        public async Task CopyToAsync(Watcher watcher, string sourceFullFileName, string sourceDirectoryName, string destDirectoryName)
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
            }
            //catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                watcher.Log(sourceFullFileName, "Copy error:" + Environment.NewLine + e.ToString());
                return;
            }
        }
    }
}
