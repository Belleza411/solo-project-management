using CommunityToolkit.Mvvm.ComponentModel;

namespace SoloProjectManagement.UI.Shell
{
	public partial class MainViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _applicationTitle = "Home";
	}
}
