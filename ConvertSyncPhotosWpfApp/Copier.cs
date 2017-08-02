using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    public class Copier
    {
        /// <summary>
        /// Method waiting for file creation
        /// </summary>
        /// <param name="fileName"></param>
        private void IsFileBusy(string fileName)
        {
            while (true)
            {
                try { using (FileStream SourceStream = File.Open(fileName, FileMode.Open)) break; }
                catch (IOException) { /*waiting*/ }
                catch (Exception) { break; }
            }
        }

        public async Task CopyToAsync(Watcher watcher, string sourceFileName, string destFileName)
        {
            IsFileBusy(sourceFileName);
            try
            {
                using (FileStream SourceStream = File.Open(sourceFileName, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destFileName))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }
                watcher.Log(sourceFileName, "Copied");
            }
            catch (Exception e)
            {
                watcher.Log(sourceFileName, "Copy error:" + Environment.NewLine + e.ToString());
                return;
            }
        }
    }
}
