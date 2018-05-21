using System;
using Prism.Navigation;
using Prism.Services;
using Realms;
using PropertyChanged;
using Valuta.Resources;

namespace Valuta.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class SettingsPageViewModel : ViewModelBase
	{
		private Realm _realm { get; }

		public SettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService,
								 IDeviceService deviceService, Realm realm)
			: base(navigationService, pageDialogService, deviceService)
		{
			_realm = realm;

			Title = AppResources.MainPageTitle;
		}
	}
}
