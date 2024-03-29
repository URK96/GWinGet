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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GWinGet.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogViewerDialog : ContentDialog
    {
        public LogViewerDialog(string fileName)
        {
            this.InitializeComponent();

            LogFileNameBlock.Text = fileName;

            LoadLogContent(fileName);
        }

        private void LoadLogContent(string logFileName)
        {
            string logFilePath = Path.Combine(Services.LogService.LogPath, logFileName);

            LogContentBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, File.ReadAllText(logFilePath));

            LogContentBox.IsReadOnly = true;
        }
    }
}
