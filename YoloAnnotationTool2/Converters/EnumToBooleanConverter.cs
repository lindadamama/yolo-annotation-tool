using System;
using System.Globalization;
using System.Windows.Data;

namespace YoloAnnotationTool2.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(Enum.Parse(value.GetType(), parameter.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Enum.Parse(targetType, parameter.ToString()) : System.Windows.Data.Binding.DoNothing;
        }
    }
}
