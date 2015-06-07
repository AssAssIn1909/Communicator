using System.Threading;
using System.Windows;

namespace TCP_Communicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MessagesManager messagesManager;

        public MainWindow()
        {
            InitializeComponent();
            messagesManager = new MessagesManager(dataGrid);
            Thread threadMessageListener = new Thread(new ThreadStart(messagesManager.Listener));

            threadMessageListener.Start();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            messagesManager.SendMessage(messageTextBox.Text);
            messageTextBox.Clear();
            this.UpdateLayout();
        }
    }
}
