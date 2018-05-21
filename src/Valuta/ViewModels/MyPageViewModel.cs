using System;
using System.Collections.Generic;
using System.Linq;
using MvvmHelpers;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Realms;
using Valuta.Models;
using Valuta.Resources;
using System.Runtime.CompilerServices;
using Valuta.Services;
using PropertyChanged;
using System.Threading.Tasks;
using Realms.Sync.Testing;

namespace Valuta.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MyPageViewModel : ViewModelBase
    {
        private Realm _realm { get; }

        public MyPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService,
                                 IDeviceService deviceService, Realm realm)
            : base(navigationService, pageDialogService, deviceService)
        {
            _realm = realm;

            Title = AppResources.MainPageTitle;

            CurrencyTappedCommand = new DelegateCommand<Currency>(OnCurrencyTappedCommandExecuted);

            AddCurrencyCommand = new DelegateCommand(OnAddCurrencyCommandExecuted);
            DeleteCurrencyCommand = new DelegateCommand<Currency>(OnDeleteCurrencyCommandExecuted);
            CurrencyTappedCommand = new DelegateCommand<Currency>(OnCurrencyTappedCommandExecuted);
            RefreshCommand = new DelegateCommand(OnRefreshCommandExecuted);
        }
        public IEnumerable<Currency> Currencies { get; set; }

        public IEnumerable<TodoItem> TodoItems { get; set; }


        public DelegateCommand AddCurrencyCommand { get; }

        public DelegateCommand<Currency> DeleteCurrencyCommand { get; }
        
        public DelegateCommand<Currency> CurrencyTappedCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            IsBusy = true;
            switch (parameters.GetNavigationMode())
            {
                case NavigationMode.Back:
                    // Do anything you want to do only when Navigating Back to the View
                    break;
                case NavigationMode.New:
					Currencies = _realm.All<Currency>().Where(cur =>
					                                          App.currentUser.MyCurrencies.Contains(cur));
                    break;
            }
            IsBusy = false;
        }

        private async void OnRefreshCommandExecuted()
        {
			IsBusy = true;
            var client = new RestClient();
            var transaction = _realm.BeginWrite();
            await client.RefreshRatesAsync(_realm);
            transaction.Commit();
            IsBusy = false;
        }

        private async void OnAddCurrencyCommandExecuted()
        {
            var transaction = _realm.BeginWrite();
			var currency = _realm.Add(new Currency(), update : true);
            await _navigationService.NavigateAsync("TodoItemDetail", new NavigationParameters
            {
                { "new", true },
                { "transaction", transaction },
                { "currency", currency }
            });
        }

        private void OnDeleteCurrencyCommandExecuted(Currency currency) =>
        _realm.Write(() => _realm.Remove(currency));

        private async void OnCurrencyTappedCommandExecuted(Currency currency) =>
            await _navigationService.NavigateAsync("TodoItemDetail", new NavigationParameters
            {
                { "currency", currency },
                { "transaction", _realm.BeginWrite() }
            });
    }
}
