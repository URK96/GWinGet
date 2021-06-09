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

namespace GWinGet.Views
{
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
