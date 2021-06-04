using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWinGet.Services
{
    public static class LogService
    {
        public static string LogPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GWinGetLog");

        public static void WriteLog(string fileName, string message)
        {
            Directory.CreateDirectory(LogPath);

#if DEBUG
            try
            {
                File.WriteAllText(Path.Combine(LogPath, string.IsNullOrWhiteSpace(fileName) ? "Log.txt" : fileName), message);
            }
            catch { }
#endif
        }

        public static void AppendLog(string fileName, string message)
        {
            Directory.CreateDirectory(LogPath);

            try
            {
                File.AppendAllText(Path.Combine(LogPath, string.IsNullOrWhiteSpace(fileName) ? "AppendLog.txt" : fileName), $"{message}\n");
            }
            catch (Exception ex)
            {
                WriteLog(string.Empty, ex.ToString());
            }
        }
    }
}
