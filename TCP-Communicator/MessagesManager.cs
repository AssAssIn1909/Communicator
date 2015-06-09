using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TCP_Communicator
{
    class MessagesManager
    {
        public ObservableCollection<Message> MessageList;
        private DataGrid dataListBox;
        MainWindow mW;

        public MessagesManager(DataGrid listbox, MainWindow _mW)
        {
            MessageList = new ObservableCollection<Message>();
            dataListBox = listbox;
            dataListBox.ItemsSource = MessageList;
            mW = _mW;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Message to display</param>
        public void AddMessage(Message message)
        {
            dataListBox.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
            {
                MessageList.Add(message);
                if (VisualTreeHelper.GetChildrenCount(dataListBox) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(dataListBox, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                    dataListBox.SelectedIndex = dataListBox.Items.Count - 1;
                }
                mW.UpdateLayout();
            }));
            
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(String message)
        {
            String[] messageArray = { Properties.Settings.Default.Nickname, message };
            Message messageToSent = new Message(messageArray[0], messageArray[1], MessageStatus.Sending);
            AddMessage(messageToSent);

            Thread t =new Thread(new ThreadStart(new Action(() =>
            {
                try
                {
                    TcpClient client = new TcpClient(Properties.Settings.Default.IPDestination, Properties.Settings.Default.Port);

                    Byte[] data = Encoding.ASCII.GetBytes(arrayToJson(messageArray));

                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);

                    dataListBox.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        int messageIndex = MessageList.IndexOf(messageToSent);
                        MessageList[messageIndex].MessageStatus = MessageStatus.Sent;
                        mW.UpdateLayout();
                    }));
                }
                catch (SocketException e)
                {
                    dataListBox.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                    {
                        int messageIndex = MessageList.IndexOf(messageToSent);
                        MessageList[messageIndex].MessageStatus = MessageStatus.Error;
                        mW.UpdateLayout();
                    }));

                   
                }
            }
            )));
            t.Start();
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void Listener()
        {
            TcpListener server = null;
            try
            {
                IPAddress listener;
                IPAddress.TryParse(Properties.Settings.Default.IPListener, out listener);
                server = new TcpListener(IPAddress.Any, Properties.Settings.Default.Port);

                server.Start();
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    int i;
                    if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        AddMessage(jsonToArray(bytes, i));
                    }
                    client.Close();
                    Thread.Sleep(10);
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK);
            }
            finally
            {
                server.Stop();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>JSON string</returns>
        public string arrayToJson(string[] message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[]));
                serializer.WriteObject(stream, message);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes">Data in Byte array</param>
        /// <param name="i">Data length</param>
        /// <returns>Message</returns>
        private Message jsonToArray(Byte[] bytes, int i)
        {
            string data = Encoding.ASCII.GetString(bytes, 0, i);
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(data)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[]));
                string[] messageString = serializer.ReadObject(memoryStream) as string[];
                return new Message(messageString[0], messageString[1],MessageStatus.Error);
            }
        }
    }
}
