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

namespace Communicator
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
            try
            {
                UdpClient client = new UdpClient(Properties.Settings.Default.IPDestination, Properties.Settings.Default.Port);

                Byte[] data = Encoding.ASCII.GetBytes(arrayToJson(messageArray));

                client.Send(data, data.Length);

                dataListBox.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                {
                    int messageIndex = MessageList.IndexOf(messageToSent);
                    MessageList[messageIndex].MessageStatus = MessageStatus.Sent;
                    mW.dataGrid.Items.Refresh();
                    //mW.UpdateLayout();
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
        
        /// <summary>
        /// 
        /// </summary>
        public void Listener()
        {
            UdpClient udpServer = new UdpClient(Properties.Settings.Default.Port);
            var remoteEP = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.Port);
            while (true)
            {
                byte[] bytes = udpServer.Receive(ref remoteEP);
                string data = Encoding.ASCII.GetString(bytes);
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(data)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string[]));
                    string[] messageString = serializer.ReadObject(memoryStream) as string[];
                    AddMessage(new Message(messageString[0], messageString[1], MessageStatus.Sent));
                }
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
