using System;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// This class is needed to convert image: resize, optimize, etc
    /// </summary>
    public class Converter
    {
        private readonly int WIDTH = 200;
        private readonly int HEIGHT = 150;
        private readonly int QUALITY = 40;

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

        public void Resize(Watcher watcher, string sourceFullFileName, string sourceDirectoryName, string destDirectoryName)
        {
            #region get destFileName
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

            //// create destination directory if exists
            //if (!Directory.Exists(destFullParentDirectory)) Directory.CreateDirectory(destFullParentDirectory);
            #endregion

            IsFileBusy(sourceFullFileName);

            try
            {
                byte[] photoBytes = File.ReadAllBytes(sourceFullFileName);
                // format is automatically detected though can be changed.
                ISupportedImageFormat format = new JpegFormat { Quality = QUALITY };
                //ISupportedImageFormat format = new WebPFormat { Quality = QUALITY }; // see: https://ru.wikipedia.org/wiki/WebP
                Size size = new Size(WIDTH, 0);
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    // initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(/*preserveExifData: true*/))
                    {
                        // load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Resize(size)
                                    .Format(format)
                                    .Save(destFileName);
                    }
                }
                watcher.Log(destFileName, $"Copied & Resized to {WIDTH}x{HEIGHT}");
            }
            catch (Exception e)
            {
                watcher.Log(destFileName, $"Copy & Resize to {WIDTH}x{HEIGHT} error:" + Environment.NewLine + e.ToString());
            }
        }

        public async Task ResizeAsync(Watcher watcher, string sourceFullFileName, string sourceDirectoryName, string destDirectoryName)
        {
            var task = new Task(() => Resize(watcher, sourceFullFileName, sourceDirectoryName, destDirectoryName));
            task.Start();
            await task;
        }
    }
}
