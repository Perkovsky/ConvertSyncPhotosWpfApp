using System;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// This class is needed to monitor changes in the directory
    /// </summary>
    public class Watcher: IDisposable
    {
        private Copier copier = new Copier();
        private ILog logger;
        private string watcherDirectory;
        private string convertDirectory;
        private FileSystemWatcher watcher;

        // because the file is written to disk in parts, i.e. event "Changed" is triggered several times to the row instead
        // of one by fixing the last name of the file being processed, we will cut off fake events
        private string lastFileName;

        public Watcher(ILog logger)
        {
            this.logger = logger;
        }

        private bool IsDirectory(string fileName)
        {
            // event "watcher.Changed" also works to delete the file / directory several times, 
            // even when the file / directory is already deleted
            try
            {
                return File.GetAttributes(fileName).HasFlag(FileAttributes.Directory);
            }
            catch (FileNotFoundException) { return true; }
            catch (DirectoryNotFoundException) { return true; }
            catch (Exception e)
            {
                Log(fileName, "IsDirectory error:" + Environment.NewLine + e.ToString());
                return true;
            }
        }

        private bool SetVariables()
        {
            // get setting from XML
            Settings settings = new Settings();

            // watcherDirectory
            string newWatcherDirectory = @settings.Fields.WatcherDirectory;
            if (!Directory.Exists(newWatcherDirectory)) return false;
            if (watcherDirectory != newWatcherDirectory || watcher == null)
            {
                watcherDirectory = newWatcherDirectory;
                watcher = new FileSystemWatcher(watcherDirectory);
                watcher.IncludeSubdirectories = true;
                //watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*";
                watcher.Changed += OnChanged;
            }

            // convertDirectory
            convertDirectory = @settings.Fields.ConvertDirectory;
            // ON/OFF logger
            logger.NeedToLog = settings.Fields.Logger;

            return true;
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            // ignore fake events
            string currentFileName = e.FullPath;
            if (IsDirectory(currentFileName)) return;
            if (!string.IsNullOrEmpty(lastFileName) && lastFileName.Equals(currentFileName)) return;
            lastFileName = currentFileName;
            //BUG: если из 1С выгружать опять последний файл, то событие не сработает

            Log(currentFileName, e.ChangeType.ToString());

            string destFileName = await copier.CopyToAsync(this, currentFileName, watcherDirectory, convertDirectory);
            //BUG: необходимо дождаться завершения копирования большего файла
        }

        public bool Start()
        {
            if (SetVariables())
            {
                watcher.EnableRaisingEvents = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

        public void Log(string fileName, string typeAction) => logger.Log(fileName, typeAction);

        public void Dispose()
        {
            watcher.Changed -= OnChanged;
        }
    }
}
