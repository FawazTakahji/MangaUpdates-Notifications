using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace MangaUpdates_Notifications.Converters
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string parameters)
                {
                    string[] parameterParts = parameters.Split(',');

                    if (parameterParts.Length >= 2)
                    {
                        string modeParameter = parameterParts[0].Trim();
                        string hideParameter = parameterParts[1].Trim();

                        // If 'modeParamater' is "normal":
                        // If 'boolValue' is true, return 'Visibility.Visible';
                        // else, return 'Visibility.Hidden' if 'hideParameter' is "hidden", or 'Visibility.Collapsed'.
                        if (modeParameter.Equals("normal", StringComparison.OrdinalIgnoreCase))
                            return boolValue ? Visibility.Visible : (hideParameter.Equals("hidden", StringComparison.OrdinalIgnoreCase) ? Visibility.Hidden : Visibility.Collapsed);
                        // If 'modeParamater' is "inverted":
                        // If 'boolValue' is true, return 'Visibility.Hidden' if 'hideParameter' is "hidden", or 'Visibility.Collapsed'.
                        // If 'boolValue' is false, return 'Visibility.Visible'.
                        else if (modeParameter.Equals("inverted", StringComparison.OrdinalIgnoreCase))
                            return boolValue ? (hideParameter.Equals("hidden", StringComparison.OrdinalIgnoreCase) ? Visibility.Hidden : Visibility.Collapsed) : Visibility.Visible;
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
