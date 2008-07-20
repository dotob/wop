using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using WOP.Tasks;

namespace WOP
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        void AppStartup(object sender, StartupEventArgs args)
        {
            Job j = Job.CreateTestJob();
            j.Start();
        }
        
    }
}
