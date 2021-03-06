﻿using System;
using System.Threading.Tasks;
using Valuta.Resources;
using Valuta.Services;
using Valuta.Views;
using Plugin.Multilingual;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Acr.UserDialogs;
using DryIoc;
using Prism.DryIoc;
using Valuta.Helpers;
using FFImageLoading.Helpers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Valuta.Helpers;
using Realms;
using Realms.Sync;
using Prism.Logging;
using Xamarin.Forms;

using DebugLogger = Valuta.Services.DebugLogger;
using System.Diagnostics;
using Newtonsoft.Json;
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace Valuta
{
    public partial class App : PrismApplication
    {
		/* 
         * NOTE: 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        
		public static Models.User currentUser = JsonConvert.DeserializeObject<Models.User>(Settings.UserJson);

        public App()
            : this(null)
        {
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
            // https://docs.microsoft.com/en-us/mobile-center/sdk/distribute/xamarin
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            // https://docs.microsoft.com/en-us/mobile-center/sdk/push/xamarin-forms
            Push.PushNotificationReceived += OnPushNotificationReceived;
            // Handle when your app starts
            AppCenter.Start(AppConstants.AppCenterStart,
                               typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
        }

        protected override async void OnInitialized()
        {
    			InitializeComponent();
    			LogUnobservedTaskExceptions();
				AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

				await NavigationService.NavigateAsync("SplashScreenPage");
        }

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register the Popup Plugin Navigation Service
			containerRegistry.RegisterPopupNavigationService();
			containerRegistry.RegisterInstance(CreateLogger());

			//var serverURL = new Uri(Secrets.RealmServer);
			//var config = new SyncConfiguration(User.Current, serverURL);
			var config = new RealmConfiguration() 
			{ 
				SchemaVersion = 0,
				ShouldDeleteIfMigrationNeeded = true,

			};

				containerRegistry.GetContainer().Register(reuse: Reuse.Transient,
								   made: Made.Of(() => Realm.GetInstance(config)),
								   setup: Setup.With(allowDisposableTransient: true));

            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);

            // Navigating to "TabbedPage?createTab=ViewA&createTab=ViewB&createTab=ViewC will generate a TabbedPage
            // with three tabs for ViewA, ViewB, & ViewC
            // Adding `selectedTab=ViewB` will set the current tab to ViewB
            containerRegistry.RegisterForNavigation<TabbedPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
			containerRegistry.RegisterForNavigation<MyPage>();
			containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<SplashScreenPage>();
            containerRegistry.RegisterForNavigation<TodoItemDetail>();
			containerRegistry.RegisterForNavigation<CustomTabbedPage>();
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            if (await Analytics.IsEnabledAsync())
            {
                System.Diagnostics.Debug.WriteLine("Analytics is enabled");
                FFImageLoading.ImageService.Instance.Config.Logger = (IMiniLogger)Container.Resolve<ILoggerFacade>();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Analytics is disabled");
            }
        }

        protected override void OnSleep()
        {
            // Handle IApplicationLifecycle
            base.OnSleep();

            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle IApplicationLifecycle
            base.OnResume();

            // Handle when your app resumes
        }

        private ILoggerFacade CreateLogger()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case "Android":
                    if (!string.IsNullOrWhiteSpace(Secrets.AppCenter_Android_Secret))
                        return CreateAppCenterLogger();
                    break;
                case "iOS":
                    if (!string.IsNullOrWhiteSpace(Secrets.AppCenter_iOS_Secret))
                        return CreateAppCenterLogger();
                    break;
            }
            return new DebugLogger();
        }

        private MCAnalyticsLogger CreateAppCenterLogger()
        {
            var logger = new MCAnalyticsLogger();
            FFImageLoading.ImageService.Instance.Config.Logger = (IMiniLogger)logger;
            return logger;
        }

        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Container.Resolve<ILoggerFacade>().Log(e.Exception);
            };
        }

        private void OnPushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message
            var summary = $"Push notification received:" +
                $"\n\tNotification title: {e.Title}" +
                $"\n\tMessage: {e.Message}";

            // If there is custom data associated with the notification,
            // print the entries
            if (e.CustomData != null)
            {
                summary += "\n\tCustom data:\n";
                foreach (var key in e.CustomData.Keys)
                {
                    summary += $"\t\t{key} : {e.CustomData[key]}\n";
                }
            }

            // Send the notification summary to debug output
            System.Diagnostics.Debug.WriteLine(summary);
            Container.Resolve<ILoggerFacade>().Log(summary);
        }

        private bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user cannot postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // Note that this method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            // Return true if you are using your own dialog, false otherwise
            return true;
        }
    }
}
