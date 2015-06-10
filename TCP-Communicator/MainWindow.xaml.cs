using System.IO;
using System.Threading;
using System.Windows;

namespace Communicator
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
            messagesManager = new MessagesManager(dataGrid, this);
            Thread threadMessageListener = new Thread(new ThreadStart(messagesManager.Listener));

            threadMessageListener.Start();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string st;
            using (StreamReader sr = new StreamReader("tttt.txt"))
            {
                st = sr.ReadToEnd();
            }
            
            messagesManager.SendMessage(st);
            messageTextBox.Clear();
            //this.UpdateLayout();
        }
    }
}
