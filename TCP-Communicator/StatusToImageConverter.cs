using System;
using System.Globalization;
using System.Windows.Data;

namespace TCP_Communicator
{
    class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageStatus mS = (MessageStatus)value;
            switch (mS)
            {
                case MessageStatus.Sent:
                    return new Uri(@"pack://application:,,,/img/sent.png");

                case MessageStatus.Sending:
                    return new Uri(@"pack://application:,,,/img/sending.gif");

                case MessageStatus.Error:
                    return new Uri(@"pack://application:,,,/img/error.png");
            }
            return new Uri(@"pack://application:,,,/img/error.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
