using SoloProjectManagement.UI.Dtos;
using SoloProjectManagement.UI.Features.Tasks;
using System.Globalization;
using System.Windows.Data;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Converters
{
	public class StatusChangeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values is [ProjectTaskDto task, TaskStatus status]
				? new StatusChange(task, status)
				: Binding.DoNothing;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
