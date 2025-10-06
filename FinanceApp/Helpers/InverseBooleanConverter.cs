using System.Globalization;

namespace FinanceApp.Helpers
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => !(value as bool? ?? false);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => !(value as bool? ?? false);
    }
}
