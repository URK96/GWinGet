using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

using AppSetting = GWinGet.Services.AppSettingService;

namespace GWinGet.Views
{
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();

            SettingNavView.SelectedItem = SettingNavView.MenuItems[0];
        }

        private void SettingNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            string tag = item.Tag as string;
            Type page = Type.GetType($"GWinGet.Views.SettingViews.{tag}");

            SettingFrame.Navigate(page);
        }
    }
}
