using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.Storage;

namespace GWinGet.Services;

public static class AppSettingService
{
    private static ApplicationDataContainer LocalSettingContainer => ApplicationData.Current.LocalSettings;

    public static void Save<T>(string key, T value)
    {
        if (LocalSettingContainer.Values.Keys.Contains(key))
        {
            LocalSettingContainer.Values[key] = value;
        }
        else
        {
            LocalSettingContainer.Values.Add(key, value);
        }
    }

    public static T Load<T>(string key, T defaultValue)
    {
        object value = LocalSettingContainer.Values[key];

        return (T)(value ?? defaultValue);
    }
}
