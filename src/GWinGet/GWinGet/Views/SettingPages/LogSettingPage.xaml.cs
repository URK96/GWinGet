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
using GWinGet.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views.SettingPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogSettingPage : Page
    {
        private string[] LogFiles => Directory.GetFiles(LogService.LogPath);

        public LogSettingPage()
        {
            this.InitializeComponent();

            CheckLogDirInfo();
        }

        private void CheckLogDirInfo()
        {
            LogStatusBlock.Text = $"Log Files : {LogFiles.Length}";
        }

        private void RemoveLogButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string path in LogFiles)
            {
                File.Delete(path);
            }

            CheckLogDirInfo();
        }
    }
}
