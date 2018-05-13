using System;
using System.Collections.Generic;
using System.Linq;
using MvvmHelpers;
using Acr.UserDialogs;
using Prism.AppModel;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using Realms;
using Valuta.Models;
using Valuta.Resources;

namespace Valuta.ViewModels
{
    public class TodoItemDetailViewModel : ViewModelBase
    {
        private IUserDialogs _userDialogs { get; }
        public TodoItemDetailViewModel(INavigationService navigationService, IPageDialogService pageDialogService,
                                       IDeviceService deviceService, IUserDialogs userDialogs)
            : base(navigationService, pageDialogService, deviceService)
        {
            _userDialogs = userDialogs;

            Title = AppResources.TodoItemDetailTitle;
            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);
        }

		public Currency Model { get; set; }

        public DelegateCommand SaveCommand { get; }

        private Transaction _transaction;

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            _transaction = parameters.GetValue<Transaction>("transaction");
			Model = parameters.GetValue<Currency>("currency");
        }

        private async void OnSaveCommandExecuted()
        {
            _transaction.Commit();
            Toast("Currency Saved");
            await _navigationService.GoBackAsync(new NavigationParameters { { "currency", Model } });
        }

        public override void Destroy()
        {
            _transaction.Dispose();
        }
        private void Toast(string message)
        {
            _userDialogs.Toast(new ToastConfig(message)
            {
                Position = ToastPosition.Top
            });
        }
    }
}