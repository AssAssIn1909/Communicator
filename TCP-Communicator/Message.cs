namespace TCP_Communicator
{
    class Message
    {
        public string Username { get; set; }
        public string TextMessage { get; set; }
        public MessageStatus MessageStatus { get; set; }

        public Message(string username, string textMessage, MessageStatus messageStatus)
        {
            Username = username;
            TextMessage = textMessage;
            MessageStatus = messageStatus;
        }
    }

    enum MessageStatus
    {
        Sent,
        Sending,
        Error
    }
}
