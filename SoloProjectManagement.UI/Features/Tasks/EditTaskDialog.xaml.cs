using SoloProjectManagement.UI.Features.Tasks.ViewModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace SoloProjectManagement.UI.Features.Tasks
{
	/// <summary>
	/// Interaction logic for EditTaskDialog.xaml
	/// </summary>
	public partial class EditTaskDialog : ContentDialog
	{
		public EditTaskDialog(EditTaskViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();

			SetResourceReference(StyleProperty, typeof(ContentDialog));
		}

		protected override void OnButtonClick(ContentDialogButton button)
		{
			if (button == ContentDialogButton.Primary
				&& DataContext is EditTaskViewModel vm
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
