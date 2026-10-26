using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace SoloProjectManagement.UI.Converters
{
	public class SpacedEnumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is null ? "" : Regex.Replace(value.ToString()!, "(?<=[a-z])(?=[A-Z])", " ");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
