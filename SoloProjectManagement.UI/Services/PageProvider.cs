using Wpf.Ui.Abstractions;

namespace SoloProjectManagement.UI.Services
{
	public class PageProvider : INavigationViewPageProvider
	{
		private readonly Dictionary<Type, Func<object>> _factories = [];
		public void Register<TPage>(Func<TPage> factory) where TPage : class
			=> _factories[typeof(TPage)] = () => factory();
		public object? GetPage(Type pageType)
			=> _factories.TryGetValue(pageType, out var factory) ? factory() : null;
	}
}
