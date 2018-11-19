using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Project_IR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MessageBox.Show("Application lunched!");
            //MainWindow main = new MainWindow();
            //main.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            MessageBox.Show("bye bye!");
        }
    }
}
