using System;
using System.Windows;

namespace Communicator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Exit(object sender, ExitEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
