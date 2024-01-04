using MangaUpdates_Notifications.Utilities;
using ModernWpf;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MangaUpdates_Notifications.Controls
{
    public partial class AccentColorPicker : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public SolidColorBrush Fill
        {
            get => (SolidColorBrush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(SolidColorBrush), typeof(AccentColorPicker));

        public AccentColorPicker()
        {
            InitializeComponent();

            foreach (var propertyInfo in typeof(Colors).GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(Color))
                {
                    Color color = (Color)propertyInfo.GetValue(null, null);

                    Button colorButton = new Button { Content = new Rectangle { Fill = new SolidColorBrush(color) }, ToolTip = propertyInfo.Name };
                    colorButton.SetValue(AutomationProperties.NameProperty, propertyInfo.Name);

                    colorButton.Click += ColorButton_Click;
                    ColorsGrid.Children.Add(colorButton);
                }
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedColor = (Button)sender;
            Rectangle rectangle = (Rectangle)clickedColor.Content;
            Color color = ((SolidColorBrush)rectangle.Fill).Color;
            ThemeManager.Current.AccentColor = color;
            ColorTextBox.Text = color.ToString();
            CurrentColor.Fill = new SolidColorBrush(color);
            ColorButton.Flyout.Hide();
        }

        private void SetColor(object sender, RoutedEventArgs e)
        {
            string colorText = ColorTextBox.Text;
            List<string> colorNames = Misc.GetColorNames();

            if (string.IsNullOrEmpty(colorText))
                return;

            if (!colorText.StartsWith('#') && !colorNames.Contains(colorText, StringComparer.OrdinalIgnoreCase))
                colorText = $"#{colorText}";

            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(colorText);
                ThemeManager.Current.AccentColor = color;
                CurrentColor.Fill = new SolidColorBrush(color);
                ColorButton.Flyout.Hide();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to set the hex color.");
            }
        }
    }
}
