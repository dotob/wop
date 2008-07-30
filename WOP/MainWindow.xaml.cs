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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // configure tasks
            var mainJob = new Job();
            mainJob.Name = "eintest";
            mainJob.AddTask(new FileGatherTask());
            mainJob.AddTask(new FileRenamerTask());
            mainJob.AddTask(new ImageShrinkTask());
            mainJob.AddTask(new GEOTagTask());
            foreach (ITask task in mainJob.TasksList) {
                if (task.UI != null) {
                    m_sp_tasks.Children.Add(task.UI);
                }
            }
            m_lb_jobs.Items.Add(new JobListTemplate {DataContext = new Job {Name = "eintest", Progress = 60}});
            m_lb_jobs.Items.Add(new JobListTemplate {DataContext = new Job {Name = "zweitest", Progress = 40}});
            m_lb_jobs.Items.Add(new JobListTemplate {DataContext = new Job {Name = "dreitest", Progress = 20}});

            Job jj = Job.CreateTestJob();
            jj.Start();
        }
    }
}