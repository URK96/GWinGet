using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;

using AppEnv = GWinGet.AppEnvironment;

namespace GWinGet.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (AppEnv.isWingetInstalled == null)
            {
                CheckWinget();
            }
            else
            {
                SetWingetCheckStatus();
            }
        }

        private async void CheckWinget()
        {
            bool isInstalled = true;

            try
            {
                StartBusy();

                var psi = new ProcessStartInfo()
                {
                    FileName = "winget",
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                psi.StandardOutputEncoding = Encoding.UTF8;

                using var p = new Process()
                {
                    StartInfo = psi
                };

                p.OutputDataReceived += (sender, e) =>
                {
                    isInstalled = false;
                };

                p.Start();
                p.BeginErrorReadLine();

                await p.WaitForExitAsync();
            }
            catch
            {
                isInstalled = false;
            }

            await Task.Delay(1000);

            AppEnv.isWingetInstalled = isInstalled;

            EndBusy();
            SetWingetCheckStatus();
        }

        private void StartBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WingetExistIcon.Visibility = Visibility.Collapsed;
                WingetNotExistIcon.Visibility = Visibility.Collapsed;
                WingetCheckRing.IsActive = true;

                WingetCheckBlock.Text = "Checking winget installation...";
            });
        }

        private void EndBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WingetCheckRing.IsActive = false;
            });
        }

        private void SetWingetCheckStatus()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (AppEnv.isWingetInstalled == true)
                {
                    WingetExistIcon.Visibility = Visibility.Visible;
                    WingetCheckBlock.Text = "Winget is installed :)";
                }
                else
                {
                    WingetNotExistIcon.Visibility = Visibility.Visible;
                    WingetCheckBlock.Text = "Winget is not installed :(";
                }
            });
        }
    }
}
