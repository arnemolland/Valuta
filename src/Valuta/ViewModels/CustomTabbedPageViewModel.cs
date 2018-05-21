using System;
using Prism.Navigation;
using Realms;
using PropertyChanged;
using Prism.Services;
using Valuta.Resources;

namespace Valuta.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class CustomTabbedPageViewModel : ViewModelBase
	{
		private Realm _realm { get; }

		public CustomTabbedPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService,
								 IDeviceService deviceService, Realm realm)
			: base(navigationService, pageDialogService, deviceService)
		{
			_realm = realm;

			Title = AppResources.MainPageTitle;
		}
	}
}
