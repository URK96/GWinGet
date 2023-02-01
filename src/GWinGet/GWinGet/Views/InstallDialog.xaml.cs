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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GWinGet.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallDialog : ContentDialog
    {
        private Package package;

        private string logFileName;

        public InstallDialog(Package package)
        {
            this.InitializeComponent();

            this.package = package;

            SetPackageInfo();
        }

        private void SetPackageInfo()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Package Name : {package.Name}")
                .AppendLine($"Package Publisher : {package.Publisher}")
                .AppendLine($"Package ID : {package.PackageId}")
                .AppendLine($"Package Ver : {package.Version}");

            PackageInfoBlock.Text = sb.ToString();
        }

        private async Task InstallProcess()
        {
            StartBusy();

            string installDateTimeStr = $"{DateTime.Now:yyyyMMddHHmmss}";

            logFileName = $"{package.Name}_Install_Log_{installDateTimeStr}.txt";

            ProcessStartInfo psi = new()
            {
                FileName = "winget",
                Arguments = $"install \"{package.Name}\"",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            psi.StandardOutputEncoding = Encoding.UTF8;

            using Process p = new()
            {
                StartInfo = psi
            };

            p.OutputDataReceived += (sender, e) =>
            {
                DispatcherQueue.TryEnqueue(() => { InstallStatus.Text = e.Data; });
                Services.LogService.AppendLog(logFileName, e.Data);
            };

            p.Start();
            p.BeginOutputReadLine();

            await p.WaitForExitAsync();

            EndBusy();
        }

        private async Task OpenLogViewer()
        {
            Hide();

            await Task.Delay(500);

            LogViewerDialog logDialog = new(logFileName)
            {
                XamlRoot = this.XamlRoot
            };

            await logDialog.ShowAsync();
        }

        private void StartBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                BusyPanel.Visibility = Visibility.Visible;
                BusyRing.IsActive = true;
                BusyStatus.Text = "Donwload & Install package...";

                InstallButton.IsEnabled = false;
                InstallButton.Visibility = Visibility.Collapsed;
                CloseButton.IsEnabled = false;
            });
        }

        private void EndBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                BusyRing.IsIndeterminate = false;
                BusyRing.Value = 100;
                BusyStatus.Text = "Finish install process";

                ViewLogButton.IsEnabled = true;
                CloseButton.IsEnabled = true;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag as string)
            {
                case "Install":
                    _ = InstallProcess();
                    break;
                case "ViewLog":
                    _ = OpenLogViewer();
                    break;
                default:
                    Hide();
                    break;
            }
        }
    }
}
