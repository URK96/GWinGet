using GWinGet.Services;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views.SettingPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralSettingPage : Page
    {
        public GeneralSettingPage()
        {
            this.InitializeComponent();

            SetInitValue();
        }

        private void SetInitValue()
        {
            ThemeSwitchComboBox.SelectedIndex = AppSettingService.Load(AppSettingKey.APP_DARKTHEME, (int)ElementTheme.Default);
        }

        private void ThemeSwitchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int before = AppSettingService.Load(AppSettingKey.APP_DARKTHEME, (int)ElementTheme.Default);

            ElementTheme theme = ((sender as ComboBox).SelectedItem as string) switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            int after = (int)theme;

            App.RootTheme = theme;
            //ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            //if (theme == ElementTheme.Dark)
            //{
            //    titleBar.ButtonForegroundColor = Colors.White;
            //}
            //else if (theme == ElementTheme.Light)
            //{
            //    titleBar.ButtonForegroundColor = Colors.Black;
            //}
            //else
            //{
            //    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            //    {
            //        titleBar.ButtonForegroundColor = Colors.White;
            //    }
            //    else
            //    {
            //        titleBar.ButtonForegroundColor = Colors.Black;
            //    }
            //}

            ThemeSwitchCaution.Visibility = (before == after) ? Visibility.Collapsed : Visibility.Visible;

            AppSettingService.Save(AppSettingKey.APP_DARKTHEME, after);
        }
    }
}
