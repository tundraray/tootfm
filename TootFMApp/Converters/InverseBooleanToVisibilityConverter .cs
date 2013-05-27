using System;
using System.Globalization;
using System.Windows;
using Coding4Fun.Toolkit.Controls.Converters;

namespace Posmotrim.TootFM.App.Converters
{
   
    public class InverseBooleanToVisibilityConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture, string language)
        {
            var boolValue = System.Convert.ToBoolean(value);

            if (parameter != null)
                boolValue = !boolValue;

            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture, string language)
        {
            return !value.Equals(Visibility.Visible);
        }
    }
}