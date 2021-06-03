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
                ps = PowerShell.Create()
                    .AddScript($"winget install {package.Name}");

                //psDataCollection = new PSDataCollection<PSObject>();
                //psDataCollection.DataAdded += (sender, e) =>
                //{
                //    File.WriteAllText(@$"C:\Users\chlwl\GWinGet\{e.Index}.txt", e.Index.ToString());
                //    InstallStatus.Text = psDataCollection[e.Index].ToString();
                //};

                ps.InvocationStateChanged += (sender, e) =>
                {
                    File.WriteAllText(@$"C:\Users\chlwl\GWinGet\State.txt", e.InvocationStateInfo.State.ToString());

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
                File.WriteAllText(@"C:\Users\chlwl\GWinGetError.txt", ex.ToString());
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
                    Arguments = $"install {package.Name}",
                    CreateNoWindow = false
                };

                var p = new Process();
                p.StartInfo = psi;

                p.Start();

                await p.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\Users\chlwl\GWinGet\Error.txt", ex.ToString());
            }

            EndBusy();
        }

        private void StartBusy()
        {
            BusyPanel.Visibility = Visibility.Visible;
            BusyRing.IsActive = true;
            BusyStatus.Text = "Donwload & Install package...";

            InstallButton.IsEnabled = false;
            CloseButton.IsEnabled = false;
        }

        private void EndBusy()
        {
            BusyRing.IsIndeterminate = false;
            BusyRing.Value = 100;
            BusyStatus.Text = "Finish install package";

            CloseButton.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag as string)
            {
                case "Install":
                    InstallProcess();
                    break;
                default:
                    Hide();
                    break;
            }
        }
    }
}
