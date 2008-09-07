using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using WOP.Tasks;

namespace WOP {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly BackgroundWorker bgSplasher = new BackgroundWorker();
        private readonly WOPSplash wops = new WOPSplash();
        private Job theJob;
        private int til = 50;

        public MainWindow()
        {
            Visibility = Visibility.Hidden;
            InitializeComponent();
            bgSplasher.WorkerReportsProgress = true;
            bgSplasher.RunWorkerCompleted += bgSplasher_RunWorkerCompleted;
            bgSplasher.DoWork += bgSplasher_DoWork;
            bgSplasher.ProgressChanged += bgSplasher_ProgressChanged;
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
            bool withSplash = false;
            if (withSplash) {
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
        }

        private void bgSplasher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wops.Close();
            Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createDefaultJob();
            addJobUIs();
        }

        private void createDefaultJob()
        {
            // configure tasks
            theJob = new Job();
            theJob.Name = "my first job";
            theJob.AddTask(new FileGatherTask { IsEnabled = true, DeleteSource = false, FilePattern = "*.jpg", RecurseDirectories = true, SourceDirectory = @"..\..\..\IM\pixweniger", TargetDirectory = @"c:\tmp" });
            theJob.AddTask(new FileRenamerTask {IsEnabled = true, RenamePattern = "bastitest_{0}"});
            theJob.AddTask(new ImageShrinkTask {IsEnabled = true, SizeX = 400, SizeY = 400, PreserveOriginals = true, NameExtension = "_thumb"});
            theJob.AddTask(new ImageRotateTask {IsEnabled = true});
            theJob.AddTask(new GEOTagTask {IsEnabled = false});
        }

        private void addJobUIs()
        {
            foreach (ITask task in theJob.TasksList) {
                if (task.UI != null) {
                    //task.UI.Margin = new Thickness(2);
                    m_sp_tasks.Children.Add(task.UI);
                }
            }
        }

        private void jj_JobFinished(object sender, EventArgs e)
        {
            MessageBox.Show("job finished");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_lb_jobs.Items.Add(theJob);
            createDefaultJob();
        }
    }
}