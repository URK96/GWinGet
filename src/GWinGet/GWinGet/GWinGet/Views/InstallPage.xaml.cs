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
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallPage : Page
    {
        public bool WorkActive { get; set; }
        public ICollection Items { get; set; }

        public InstallPage()
        {
            this.InitializeComponent();

            SearchPackages();

            var task = new Task(SearchPackages);

            task.Start();
        }

        private void SearchPackages()
        {
            WorkActive = true;

            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("winget");
                ps.AddArgument("search");

                var list = ps.Invoke();

                var item = list[0];

                LV.ItemsSource = list;
            }

            WorkActive = false;
        }
    }
}
