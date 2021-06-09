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

using AppEnv = GWinGet.AppEnvironment;

namespace GWinGet
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.Title = "GWinGet";

            MainNavView.SelectedItem = MainNavView.MenuItems[0];
        }

        private void MainNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                sender.Header = item.Content;

                MainFrame.Navigate(typeof(Views.SettingPage));
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                string tag = item.Tag as string;
                Type page = Type.GetType($"GWinGet.Views.{tag}");
                object arg = null;

                sender.Header = item.Content;

                if ((AppEnv.isWingetInstalled != true) &&
                    (tag.Equals("InstallPage") || tag.Equals("ManagePage")))
                {
                    page = typeof(Views.ErrorPage);
                    arg = "Winget is not installed";
                }

                MainFrame.Navigate(page, arg);
            }
        }
    }
}
