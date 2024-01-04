using System;
using System.Globalization;
using System.Windows.Data;

namespace MangaUpdates_Notifications.Converters
{
    public class PageToBoolean : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is int page && values[1] is int currentPage)
            {
                return page != currentPage;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
