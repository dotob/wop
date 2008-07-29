using System.Windows;
using WOP.Tasks;

namespace WOP {
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {
        private void AppStartup(object sender, StartupEventArgs args)
        {
            Job j = Job.CreateTestJob();
            j.Start();
        }
    }
}