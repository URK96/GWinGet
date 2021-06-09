using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace GWinGet.Services
{
    public static class AppSettingService
    {
        public static ApplicationDataContainer LocalSettingContainer => ApplicationData.Current.LocalSettings;

        public static void Save<T>(string key, T value) => LocalSettingContainer.Values.Add(key, value);
        
        public static T Load<T>(string key, T defaultValue)
        {
            try
            {
                return (T)LocalSettingContainer.Values[key];
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
