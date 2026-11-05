using SoloProjectManagement.UI.Features.Projects.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace SoloProjectManagement.UI.Features.Projects
{
	/// <summary>
	/// Interaction logic for AddProjectDialog.xaml
	/// </summary>
	public partial class AddProjectDialog : ContentDialog
	{
		public AddProjectDialog(ContentPresenter? host, AddProjectViewModel viewModel) : base(host)
		{
			DataContext = viewModel;
			InitializeComponent();

			SetResourceReference(StyleProperty, typeof(ContentDialog));
		}

		protected override void OnButtonClick(ContentDialogButton button)
		{
			if (button == ContentDialogButton.Primary
				&& DataContext is AddProjectViewModel vm
				&& !vm.IsValid())
			{
				return;
			}

			base.OnButtonClick(button);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
			=> OnButtonClick(ContentDialogButton.Close);

		private void Cancel_Click(object sender, RoutedEventArgs e)
			=> OnButtonClick(ContentDialogButton.Close);

		private void Create_Click(object sender, RoutedEventArgs e)
			=> OnButtonClick(ContentDialogButton.Primary);
	}
}
