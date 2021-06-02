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

namespace GWinGet.Views
{
    public sealed partial class InstallPage : Page
    {
        public Services.PackageDBService PackageDBService { get; }

        public InstallPage()
        {
            this.InitializeComponent();

            PackageDBService = new Services.PackageDBService();
        }

        private async void RefreshPackages()
        {
            BusyPanel.Visibility = Visibility.Visible;
            BusyRing.IsActive = true;
            BusyStatus.Text = "Update WinGet DB";
            PackageDataGrid.IsEnabled = false;

            using var ps = PowerShell.Create();

            ps.AddCommand("winget");
            ps.AddArgument("search");

            _ = await ps.InvokeAsync();

            BusyStatus.Text = "List packages";

            await Task.Run(() =>
            {
                PackageDBService.RefreshList();

                DispatcherQueue.TryEnqueue(() => { PackageDataGrid.ItemsSource = PackageDBService.AvailablePackages; });
            });

            BusyPanel.Visibility = Visibility.Collapsed;
            BusyRing.IsActive = false;
            PackageDataGrid.IsEnabled = true;

            //File.WriteAllText(@"C:\Users\URK96\GWinGetError.txt", PackageDBService.AvailablePackages.Last().Name);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPackages();
        }

        private void PackageDataGrid_AutoGeneratingColumn(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
