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
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;

using AppEnv = GWinGet.AppEnvironment;
using GWinGet.Models;

namespace GWinGet.Views
{
    public sealed partial class ManagePage : Page
    {
        private Services.InstalledAppManager AppManager => AppEnv.installedAppManager;

        public ManagePage()
        {
            this.InitializeComponent();

            if (AppManager == null)
            {
                AppEnv.installedAppManager = new Services.InstalledAppManager();

                RefreshInstalledList();
            }
            else
            {
                FilterPackages();
            }
        }

        private async void RefreshInstalledList()
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
            var queryString = PackageSearchBox.Text;

            var resultList = !string.IsNullOrWhiteSpace(queryString) ?
                AppManager.InstalledPackages.FindAll(x => x.Name.Contains(queryString, StringComparison.OrdinalIgnoreCase)) :
                AppManager.InstalledPackages;

            RefreshListUI(resultList);
        }

        private void RefreshListUI(List<Package> list)
        {
            PackageDataGrid.ItemsSource = list;

            PackageCountBlock.Text = $"Packages : {AppManager.InstalledPackages.Count}";
        }

        private async void ShowUninstallDialog(Package package)
        {
            var dialog = new UninstallDialog(package)
            {
                XamlRoot = Content.XamlRoot
            };

            dialog.Closed += delegate { RefreshInstalledList(); };

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
                        ShowUninstallDialog(PackageDataGrid.SelectedItem as Package);
                    }
                    break;
                case "Refresh":
                    RefreshInstalledList();
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
    }
}
