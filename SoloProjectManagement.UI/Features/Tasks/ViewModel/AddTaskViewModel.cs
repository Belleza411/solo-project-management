using CommunityToolkit.Mvvm.ComponentModel;
using SoloProjectManagement.UI.Models;
using System.ComponentModel.DataAnnotations;
using TaskStatus = SoloProjectManagement.UI.Models.TaskStatus;

namespace SoloProjectManagement.UI.Features.Tasks.ViewModel
{
	public partial class AddTaskViewModel : ObservableValidator
	{
		[
			ObservableProperty,
			NotifyDataErrorInfo,
			Required(ErrorMessage = "Please enter task name"),
			MaxLength(100)
		]
		private string _name = string.Empty;

		[ObservableProperty]
		private string _description = string.Empty;

		[ObservableProperty, Required(ErrorMessage = "Please selected a status")]
		private TaskStatus _status = TaskStatus.Todo;

		[ObservableProperty, Required(ErrorMessage = "Please selected a priority")]
		private TaskPriority _priority = TaskPriority.Low;

		[ObservableProperty]
		private DateTime? _dueDate;
		public IEnumerable<TaskStatus> Statuses { get; } = Enum.GetValues<TaskStatus>();
		public IEnumerable<TaskPriority> Priorities { get; } = Enum.GetValues<TaskPriority>();

		public bool IsValid()
		{
			ValidateAllProperties();
			return !HasErrors;
		}
	}
}
