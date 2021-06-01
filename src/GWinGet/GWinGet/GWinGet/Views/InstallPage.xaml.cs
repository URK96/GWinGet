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
        public InstallPage()
        {
            this.InitializeComponent();
        }

        private async void RefreshPackages()
        {
            using var ps = PowerShell.Create();

            ps.AddCommand("winget");
            ps.AddArgument("search");

            await ps.InvokeAsync();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPackages();
        }
    }
}
