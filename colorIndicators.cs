using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AT1_Sensor
{
    public class ColorIndicators : IValueConverter
    {
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !double.TryParse(value.ToString(), out double val))
                return Brushes.Black;

            if (val < LowerBound)
                return Brushes.Blue;
            else if (val > UpperBound)
                return Brushes.Red;
            else
                return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
