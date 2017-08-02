using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    public class Watcher
    {
        private Copier copier = new Copier();
        private ILog logger;
        private string watcherDirectory;
        private string convertDirectory;
        private FileSystemWatcher watcher;

        // because the file is written to disk in parts, i.e. event "Changed" is triggered several times to the row instead
        // of one by fixing the last name of the file being processed and the modification date, we will cut off fake events
        private string lastFileName;
        private DateTime lastFileDateModified;

        public Watcher(ILog logger)
        {
            this.logger = logger;
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
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*";

                watcher.Changed += async (object sender, FileSystemEventArgs e) =>
                {
                    //BUG: на созданный файл создается как минимум два события (Created-Changed), а то и три (Created-Changed-Changed)
                    // ignore fake events
                    string currentFileName = e.FullPath;
                    DateTime currentFileDateModified = File.GetLastWriteTime(currentFileName); // File.GetCreationTime(currentFileName)
                    if (!string.IsNullOrEmpty(lastFileName)
                        && lastFileName.Equals(currentFileName))
                        //TODO: добавить условие по последней дате модификации или обнулять lastFileName после завершения копирования
                        //&&
                        return;

                    lastFileName = currentFileName;
                    //lastFileDateModified = currentFileDateModified;

                    Log(currentFileName, e.ChangeType.ToString());

                    //NOTE: это важно! продумать правильную логику приложения
                    // возможно нужна будет оптимизация - групповое копирование и конвертирование фото 
                    //FileConverting.Convert(this, currentFileName, string.Format(@"{0}{1}", convertDirectory, Path.GetFileName(currentFileName)));
                    await copier.CopyToAsync(this, currentFileName, string.Format(@"{0}{1}", convertDirectory, Path.GetFileName(currentFileName)));
                    //BUG: необходимо дождаться завершения копирования большего файла
                };
            }

            // convertDirectory
            convertDirectory = @settings.Fields.ConvertDirectory;
            // ON/OFF logger
            logger.NeedToLog = settings.Fields.Logger;

            return true;
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
    }
}
