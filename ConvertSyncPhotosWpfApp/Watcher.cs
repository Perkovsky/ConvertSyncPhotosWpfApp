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
        private readonly string LOG_FILE_NAME = string.Format(@"{0}\log.txt", FileConverting.GetCurrentDirectory());

        public event FileSystemEventHandler Changed = null;

        private string watcherDirectory;
        private string convertDirectory;
        private bool logger;
        private FileInfo file;
        private StreamWriter writer;
        private FileSystemWatcher watcher;

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

                //BUG: на созданный файл создается как минимум два события (Created-Changed), а то и три (Created-Changed-Changed)
                //watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*"; //TODO: filter

                watcher.Created += WatcherChanged;
                watcher.Changed += WatcherChanged;
                //watcher.Deleted += WatcherChanged;
                //watcher.Renamed += WatcherChanged;
            }

            // convertDirectory
            convertDirectory = @settings.Fields.ConvertDirectory;

            // logger
            logger = settings.Fields.Logger;
            if (logger && writer == null)
            {
                file = new FileInfo(LOG_FILE_NAME);
                writer = file.AppendText();
            }
            else if (!logger && writer != null)
            {
                writer.Close();
                writer = null;
            }

            return true;
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            Log(string.Format("{0} -> {1}", Path.GetFileName(e.FullPath), e.ChangeType));

            Changed.Invoke(sender, e);

            //NOTE: это важно! продумать правильную логику приложения
            // возможно нужна будет оптимизация - групповое копирование и конвертирование фото 
            //TODO: async - await
            FileConverting.Convert(this, e.FullPath, string.Format(@"{0}{1}", convertDirectory, Path.GetFileName(e.FullPath)));
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

        public void Log(string msg)
        {
            if (!logger) return;

            writer.WriteLine("{0} :: {1}", DateTime.Now.ToString(), msg);
            writer.Flush();
        }
    }
}
