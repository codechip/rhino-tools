using System;
using System.Globalization;
using System.Windows.Data;

namespace Browser
{
	public class StringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string format = parameter as string;
			return string.Format(culture, format, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}