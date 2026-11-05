using CommunityToolkit.Mvvm.ComponentModel;
using SoloProjectManagement.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace SoloProjectManagement.UI.Features.Projects.ViewModels
{
	public partial class AddProjectViewModel : ObservableValidator
	{
		[
			ObservableProperty,
			NotifyDataErrorInfo,
			Required(ErrorMessage = "Please enter project title"),
			MaxLength(100)
		]
		private string _title = string.Empty;

		[ObservableProperty]
		private string _description = string.Empty;

		[ObservableProperty, Required(ErrorMessage = "Please select a category")]
		private ProjectCategory _category = ProjectCategory.Other;

		[ObservableProperty, Required(ErrorMessage = "Please select a status")]
		private ProjectStatus _status = ProjectStatus.NotStarted;

		[ObservableProperty]
		private DateTime? _dueDate;

		public IEnumerable<ProjectStatus> Statuses { get; } = Enum.GetValues<ProjectStatus>();
		public IEnumerable<ProjectCategory> Categories { get; } = Enum.GetValues<ProjectCategory>();

		public bool IsValid()
		{
			ValidateAllProperties();
			return !HasErrors;
		}
	}
}
