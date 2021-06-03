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

namespace GWinGet.Views
{
    public sealed partial class InstallPage : Page
    {
        public Services.PackageDBService PackageDBService { get; }

        public InstallPage()
        {
            this.InitializeComponent();

            PackageDBService = new Services.PackageDBService();

            RefreshPackages();
        }

        private async void RefreshPackages()
        {
            StartBusy();

            BusyStatus.Text = "Update WinGet DB";

            using var ps = PowerShell.Create();

            ps.AddCommand("winget");
            ps.AddArgument("search");

            _ = await ps.InvokeAsync();

            BusyStatus.Text = "List packages";

            await Task.Run(() =>
            {
                try
                {
                    PackageDBService.RefreshList();

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        FilterPackages();

                        DBVersionBlock.Text = $"DB Ver : {PackageDBService.DBVersion}";
                    });
                }
                catch (Exception ex)
                {
                    File.WriteAllText(@"C:\Users\chlwl\GWinGet\RefreshListError.txt", ex.ToString());
                }
            });

            EndBusy();
        }

        private void FilterPackages()
        {
            var queryString = PackageSearchBox.Text;

            PackageDataGrid.ItemsSource = !string.IsNullOrWhiteSpace(queryString) ?
                PackageDBService.AvailablePackages.FindAll(x => x.Name.Contains(queryString, StringComparison.OrdinalIgnoreCase)) :
                PackageDBService.AvailablePackages;
        }

        private async void RunInstall(Package package)
        {
            var dialog = new InstallDialog(package)
            {
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void StartBusy()
        {
            BusyPanel.Visibility = Visibility.Visible;
            BusyRing.IsActive = true;
            
            PackageDataGrid.Visibility = Visibility.Collapsed;
            PackageListCommandBar.IsEnabled = false;
            PackageSearchBox.IsEnabled = false;
        }

        private void EndBusy()
        {
            BusyPanel.Visibility = Visibility.Collapsed;
            BusyRing.IsActive = false;

            PackageDataGrid.Visibility = Visibility.Visible;
            PackageListCommandBar.IsEnabled = true;
            PackageSearchBox.IsEnabled = true;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as AppBarButton).Tag as string)
            {
                case "Install":
                    RunInstall(PackageDataGrid.SelectedItem as Package);
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
