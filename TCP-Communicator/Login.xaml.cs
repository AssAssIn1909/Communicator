using System.Windows;

namespace Communicator
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IPDestination = destinationTextBox.Text;
            //Properties.Settings.Default.IPListener = listenerTextBox.Text;
            Properties.Settings.Default.Nickname = nicknameTextBox.Text;

            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}
