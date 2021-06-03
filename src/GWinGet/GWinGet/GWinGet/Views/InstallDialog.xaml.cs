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

using GWinGet.Models;
using System.Text;
using System.Management.Automation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GWinGet.Views
{
    public sealed partial class InstallDialog : ContentDialog
    {
        private Package package;

        private PowerShell ps;
        private PSDataCollection<PSObject> psDataCollection;

        public InstallDialog(Package package)
        {
            this.InitializeComponent();

            this.package = package;

            SetPackageInfo();
        }

        private void SetPackageInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Package Name : {package.Name}")
                .AppendLine($"Package Publisher : {package.Publisher}")
                .AppendLine($"Package ID : {package.PackageId}")
                .AppendLine($"Package Ver : {package.Version}");

            PackageInfoBlock.Text = sb.ToString();
        }

        private async void InstallProcess()
        {
            StartBusy();

            try
            {
                using var ps = PowerShell.Create()
                    .AddScript($"winget install {package.Name}");

                //psDataCollection = new PSDataCollection<PSObject>();
                //psDataCollection.DataAdded += (sender, e) =>
                //{
                //    InstallStatus.Text = psDataCollection[e.Index].ToString();
                //};

                ps.InvocationStateChanged += (sender, e) =>
                {

                    if (e.InvocationStateInfo.State is
                        PSInvocationState.Completed or
                        PSInvocationState.Stopped)
                    {
                        DispatcherQueue.TryEnqueue(() => { EndBusy(); });

                        ps?.Dispose();
                    }
                };

                //_ = await ps.InvokeAsync(new PSDataCollection<PSObject>(), psDataCollection);
                _ = await ps.InvokeAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async void InstallProcessAlt()
        {
            StartBusy();

            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = "winget",
                };
                psi.StandardOutputEncoding = Encoding.UTF8;

                var p = new Process();
                p.StartInfo = psi;

                p.OutputDataReceived += (sender, e) =>
                {
                    //File.AppendAllText(@$"C:\Users\URK96\GWinGetLog\{package.Name}_Install_Log.txt", $"{e.Data}\n");
                    DispatcherQueue.TryEnqueue(() => { InstallStatus.Text = e.Data; });
                };

                p.Start();
                p.BeginOutputReadLine();

                await p.WaitForExitAsync();
            }
            catch (Exception ex)
            {
            }

            EndBusy();
        }

        private void StartBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
            BusyPanel.Visibility = Visibility.Visible;
            BusyRing.IsActive = true;
            BusyStatus.Text = "Donwload & Install package...";

            InstallButton.IsEnabled = false;
            CloseButton.IsEnabled = false;
            });
        }

        private void EndBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
            BusyRing.IsIndeterminate = false;
            BusyRing.Value = 100;

            CloseButton.IsEnabled = true;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag as string)
            {
                case "Install":
                    break;
                default:
                    Hide();
                    break;
            }
        }
    }
}
