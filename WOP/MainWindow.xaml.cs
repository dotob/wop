using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace WOP {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private BackgroundWorker bgSplasher = new BackgroundWorker();
        private int til = 50;
        private WOPSplash wops = new WOPSplash();

        public MainWindow()
        {
            Visibility = Visibility.Hidden;
            InitializeComponent();
            bgSplasher.WorkerReportsProgress = true;
            bgSplasher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgSplasher_RunWorkerCompleted);
            bgSplasher.DoWork += new DoWorkEventHandler(bgSplasher_DoWork);
            bgSplasher.ProgressChanged += new ProgressChangedEventHandler(bgSplasher_ProgressChanged);
            wops.Opacity = 0;
            wops.Show();
            bgSplasher.RunWorkerAsync();
        }

        private void bgSplasher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            wops.Opacity = 1f/til*e.ProgressPercentage;
        }

        private void bgSplasher_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < til; i++) {
                bgSplasher.ReportProgress(i);
                Thread.Sleep(10);
            }
            Thread.Sleep(2000);
            for (int i = til/2; i >= 0; i--) {
                bgSplasher.ReportProgress(i*2);
                Thread.Sleep(1);
            }
        }

        private void bgSplasher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wops.Close();
            Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {}
    }
}