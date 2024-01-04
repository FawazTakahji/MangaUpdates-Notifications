using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MangaUpdates_Notifications.Converters
{
    public class StringToVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && !string.IsNullOrEmpty(strValue))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
