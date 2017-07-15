﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Watcher watcher = new Watcher();

        private void SetButtonsAvailability(bool isPressStart = true)
        {
            btnStart.IsEnabled = !isPressStart;
            btnStop.IsEnabled = isPressStart;
        }

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
