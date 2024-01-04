using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MangaUpdates_Notifications.Converters
{
    public class StringToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
                return !string.IsNullOrEmpty(strValue);
            else if (value is null)
                return false;

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
