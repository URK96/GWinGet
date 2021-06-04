﻿using Microsoft.UI.Xaml;
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

        private string logFileName;

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

            var installDateTimeStr = $"{DateTime.Now:yyyyMMddHHmmss}";

            logFileName = $"{package.Name}_Install_Log_{installDateTimeStr}.txt";

            var psi = new ProcessStartInfo()
            {
                FileName = "winget",
                Arguments = $"install \"{package.Name}\"",
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            psi.StandardOutputEncoding = Encoding.UTF8;

            var p = new Process();
            p.StartInfo = psi;

            p.OutputDataReceived += (sender, e) =>
            {
                Services.LogService.AppendLog(logFileName, e.Data);
                DispatcherQueue.TryEnqueue(() => { InstallStatus.Text = e.Data; });
            };

            p.Start();
            p.BeginOutputReadLine();

            await p.WaitForExitAsync();

            EndBusy();
        }

        private async void OpenLogViewer()
        {
            Hide();

            await Task.Delay(500);

            try
            {
                var logDialog = new LogViewerDialog(logFileName)
                {
                    XamlRoot = this.XamlRoot
                };

                await logDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Services.LogService.WriteLog("LogViewerError.txt", ex.ToString());
            }
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
                    InstallProcess();
                    break;
                case "ViewLog":
                    OpenLogViewer();
                    break;
                default:
                    Hide();
                    break;
            }
        }
    }
}
