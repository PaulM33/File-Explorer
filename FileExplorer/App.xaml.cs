using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow main_;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            main_ = new MainWindow();
            main_.Show();
        }

        public static new App Current
        {
            get { return Application.Current as App; }
        }

        public MainWindow main { get { return main_; } }
    }
}
