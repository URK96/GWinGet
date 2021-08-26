using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        private bool hasUpdate = false;

        public InfoPage()
        {
            this.InitializeComponent();

            SetInfo();

            _ = CheckUpdate();
        }

        private void SetInfo()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;

            AppVersionBlock.Text = $"v{packageId.Version.Major}.{packageId.Version.Minor}.{packageId.Version.Build}";

            string githubIconURI = Application.Current.RequestedTheme == ApplicationTheme.Light ?
                "ms-appx:///Assets/github_icon.png" :
                "ms-appx:///Assets/github_light_icon.png";

            GithubIcon.Source = new BitmapImage(new Uri(githubIconURI));
        }

        private async Task CheckUpdate()
        {
            StartBusy();



            EndBusy();

            SetUpdateCheckStatus();
        }

        private void StartBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WingetHasUpdateIcon.Visibility = Visibility.Collapsed;
                WingetUpToDateIcon.Visibility = Visibility.Collapsed;
                WingetUpdateCheckRing.IsActive = true;

                WingetUpdateCheckBlock.Text = "Checking GWinGet update...";
            });
        }

        private void EndBusy()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                WingetUpdateCheckRing.IsActive = false;
            });
        }

        private void SetUpdateCheckStatus()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (hasUpdate)
                {
                    WingetHasUpdateIcon.Visibility = Visibility.Visible;
                    WingetUpToDateIcon.Visibility = Visibility.Collapsed;
                    WingetUpdateCheckBlock.Text = "GWinGet update is ready";
                }
                else
                {
                    WingetHasUpdateIcon.Visibility = Visibility.Collapsed;
                    WingetUpToDateIcon.Visibility = Visibility.Visible;
                    WingetUpdateCheckBlock.Text = "GWinGet is up to date";
                }
            });
        }
    }
}
