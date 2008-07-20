using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WOP
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker bgSplasher = new BackgroundWorker ();
        private WOPSplash wops = new WOPSplash ();
        private int til = 50;

        public MainWindow () {
            this.Visibility = Visibility.Hidden;
            InitializeComponent ();
            bgSplasher.WorkerReportsProgress = true;
            bgSplasher.RunWorkerCompleted += new RunWorkerCompletedEventHandler ( bgSplasher_RunWorkerCompleted );
            bgSplasher.DoWork += new DoWorkEventHandler ( bgSplasher_DoWork );
            bgSplasher.ProgressChanged += new ProgressChangedEventHandler ( bgSplasher_ProgressChanged );
            wops.Opacity = 0;
            wops.Show ();
            bgSplasher.RunWorkerAsync ();
        }

        void bgSplasher_ProgressChanged ( object sender, ProgressChangedEventArgs e ) {
            wops.Opacity = 1f / til * e.ProgressPercentage;
        }

        void bgSplasher_DoWork ( object sender, DoWorkEventArgs e ) {
            for (int i = 0; i < til; i++) {
                bgSplasher.ReportProgress ( i );
                Thread.Sleep ( 10 );
            }
            Thread.Sleep ( 1000 );
            for (int i = til / 2; i >= 0; i--) {
                bgSplasher.ReportProgress ( i * 2 );
                Thread.Sleep ( 1 );
            }
        }

        void bgSplasher_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e ) {
            wops.Close ();
            this.Visibility = Visibility.Visible;
        }

        private void Window_Loaded ( object sender, System.Windows.RoutedEventArgs e ) {

        }

    }
}
