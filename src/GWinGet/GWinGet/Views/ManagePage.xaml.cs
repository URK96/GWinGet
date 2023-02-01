// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

using AppEnv = GWinGet.AppEnvironment;
using GWinGet.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManagePage : Page
    {
        private Services.InstalledAppManager AppManager => AppEnv.installedAppManager;

        public ManagePage()
        {
            this.InitializeComponent();

            if (AppManager == null)
            {
                AppEnv.installedAppManager = new Services.InstalledAppManager();

                _ = RefreshInstalledList();
            }
            else
            {
                FilterPackages();
            }
        }

        private async Task RefreshInstalledList()
        {
            StartBusy();

            BusyStatus.Text = "List installed packages";

            await Task.Delay(500);

            await Task.Run(() =>
            {
                try
                {
                    AppManager.RefreshList();

                    DispatcherQueue.TryEnqueue(() => { FilterPackages(); });
                }
                catch (Exception ex)
                {
                    Services.LogService.WriteLog("ListPackagesError.txt", ex.ToString());
                }
            });

            EndBusy();
        }

        private void FilterPackages()
        {
            string queryString = PackageSearchBox.Text;

            List<Package> resultList = !string.IsNullOrWhiteSpace(queryString) ?
                AppManager.InstalledPackages.FindAll(x => x.Name.Contains(queryString, StringComparison.OrdinalIgnoreCase)) :
                AppManager.InstalledPackages;

            RefreshListUI(resultList);
        }

        private void RefreshListUI(List<Package> list)
        {
            PackageDataGrid.ItemsSource = list;

            PackageCountBlock.Text = $"Packages : {AppManager.InstalledPackages.Count}";

            GC.Collect();
        }

        private async Task ShowUninstallDialog(Package package)
        {
            UninstallDialog dialog = new(package)
            {
                XamlRoot = Content.XamlRoot
            };

            dialog.Closed += async delegate { await RefreshInstalledList(); };

            await dialog.ShowAsync();
        }

        private void StartBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                BusyPanel.Visibility = Visibility.Visible;
                BusyRing.IsActive = true;

                PackageDataGrid.Visibility = Visibility.Collapsed;
                PackageListCommandBar.IsEnabled = false;
                PackageSearchBox.IsEnabled = false;
            });
        }

        private void EndBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                BusyPanel.Visibility = Visibility.Collapsed;
                BusyRing.IsActive = false;

                PackageDataGrid.Visibility = Visibility.Visible;
                PackageListCommandBar.IsEnabled = true;
                PackageSearchBox.IsEnabled = true;
            });
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as AppBarButton).Tag as string)
            {
                case "Uninstall":
                    if (PackageDataGrid.SelectedItems.Count > 0)
                    {
                        _ = ShowUninstallDialog(PackageDataGrid.SelectedItem as Package);
                    }
                    break;
                case "Refresh":
                    _ = RefreshInstalledList();
                    break;
                default:
                    break;
            }
        }

        private void PackageDataGrid_AutoGeneratingColumn(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = CommunityToolkit.WinUI.UI.Controls.DataGridLength.SizeToHeader;
            e.Column.MinWidth = 300;
        }

        private void PackageSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            FilterPackages();
        }

        // Protect app crash cause memory
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PackageDataGrid.ItemsSource = null;
        }
    }
}
