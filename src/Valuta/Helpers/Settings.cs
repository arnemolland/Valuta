using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Newtonsoft.Json;
using Valuta.Models;

namespace Valuta.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
{
    private static ISettings AppSettings
    {
        get
        {
            return CrossSettings.Current;
        }
    }

    #region Setting Constants

    private const string SettingsKey = "settings_key";
    private static readonly string SettingsDefault = string.Empty;
    
		private const string UserKey = "user_key";
		private static readonly string UserJsonDefault = JsonConvert.SerializeObject(new User());

		private const string TrendKey = "trend_key";
		private static readonly string TrendDefault = "B";

    #endregion


    public static string GeneralSettings
    {
        get
        {
            return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
        }
        set
        {
            AppSettings.AddOrUpdateValue(SettingsKey, value);
        }
    }

        public static string UserJson
		{
			get
			{
				return AppSettings.GetValueOrDefault(UserKey, UserJsonDefault);
			}
            
			set
			{
				AppSettings.AddOrUpdateValue(UserKey, JsonConvert.SerializeObject(value));
			}
		}

        public static string SelectedTrend
		{
            get
			{
				return AppSettings.GetValueOrDefault(TrendKey, TrendDefault);
			}

            set
			{
				AppSettings.AddOrUpdateValue(TrendKey, value);
			}
		}

}
}
