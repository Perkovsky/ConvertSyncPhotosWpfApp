﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    public class Watcher
    {
        private readonly string LOG_FILE_NAME = string.Format(@"{0}\log.txt", GeneralMethods.GetCurrentDirectory());

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
                // после применения NotifyFilter, вроде бы работает, необходимо тщательное тестирование
                watcher.NotifyFilter = NotifyFilters.LastWrite;
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
            Log(string.Format("{0} -> {1}", e.Name, e.ChangeType));

            //TODO: copy file-photo to preview, convert file-photo into preview
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

        private void Log(string log)
        {
            if (!logger) return;

            writer.WriteLine("{0} :: {1}", DateTime.Now.ToString(), log);
            writer.Flush();
        }
    }
}
