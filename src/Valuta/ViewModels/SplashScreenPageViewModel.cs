using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;
using Valuta.Helpers;
using Valuta.Services;
using Realms;

namespace Valuta.ViewModels
{

    public class SplashScreenPageViewModel : ViewModelBase
    {
		public SplashScreenPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDeviceService deviceService)
            : base(navigationService, pageDialogService, deviceService)
        {
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
			await _navigationService.NavigateAsync("/NavigationPage/CustomTabbedPage?selectedTab=MainPage");
        }
        
    }
}