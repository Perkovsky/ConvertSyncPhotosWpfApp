﻿using System;
using System.Windows;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILog
    {
        private readonly Watcher watcher;
        private bool needToLog;
        public bool NeedToLog { set { needToLog = value; } }

        public void Log(string fileName, string typeAction)
        {
            Dispatcher.Invoke(new Action(() =>
                tbLog.AppendText(Logger.FormatMsg(fileName, typeAction + Environment.NewLine)))
            );
        }

        private void SetButtonsAvailability(bool isPressStart = true)
        {
            btnStart.IsEnabled = !isPressStart;
            btnStop.IsEnabled = isPressStart;
        }

        public MainWindow()
        {
            InitializeComponent();
            watcher = new Watcher(this);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (watcher.Start())
            {
                SetButtonsAvailability();
            }
            else
            {
                MessageBox.Show("Goto \"settings.xml\" and specify settings.");
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (watcher == null)
            {
                MessageBox.Show("Watcher did not started!");
                return;
            }
            else
            {
                watcher.Stop();
            }
            SetButtonsAvailability(false);
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e) => tbLog.Clear();
    }
}
