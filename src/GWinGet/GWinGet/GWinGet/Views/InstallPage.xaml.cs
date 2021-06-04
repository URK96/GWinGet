using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;

using GWinGet.Models;
using System.Diagnostics;

using AppEnv = GWinGet.AppEnvironment;
using System.Text;

namespace GWinGet.Views
{
    public sealed partial class InstallPage : Page
    {
        private Services.PackageDBService PackageDBService => AppEnv.packageDBService;

        public InstallPage()
        {
            this.InitializeComponent();

            if (PackageDBService == null)
            {
                AppEnv.packageDBService = new Services.PackageDBService();

                RefreshPackages();
            }
            else
            {
                FilterPackages();
            }
        }

        private async void RefreshPackages()
        {
            StartBusy();

            BusyStatus.Text = "Update WinGet DB";

            var psi = new ProcessStartInfo()
            {
                FileName = "winget",
                Arguments = $"search",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            psi.StandardOutputEncoding = Encoding.UTF8;

            using var p = new Process()
            {
                StartInfo = psi
            };

            p.Start();
            p.BeginOutputReadLine();

            await p.WaitForExitAsync();

            BusyStatus.Text = "List packages";

            await Task.Run(() =>
            {
                try
                {
                    PackageDBService.RefreshList();

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
                PackageDBService.AvailablePackages.FindAll(x => x.Name.Contains(queryString, StringComparison.OrdinalIgnoreCase)) :
                PackageDBService.AvailablePackages;

            RefreshListUI(resultList);
        }

        private void RefreshListUI(List<Package> list)
        {
            PackageDataGrid.ItemsSource = list;

            DBVersionBlock.Text = $"DB Ver : {PackageDBService.DBVersion}";
        }

        private async void ShowInstallDialog(Package package)
        {
            var dialog = new InstallDialog(package)
            {
                XamlRoot = Content.XamlRoot
            };

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
                case "Install":
                    if (PackageDataGrid.SelectedItems.Count > 0)
                    {
                        ShowInstallDialog(PackageDataGrid.SelectedItem as Package);
                    }
                    break;
                case "Refresh":
                    RefreshPackages();
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
