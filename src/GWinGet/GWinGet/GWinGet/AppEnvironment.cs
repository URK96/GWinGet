using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GWinGet.Services;

namespace GWinGet
{
    public static class AppEnvironment
    {
        public static PackageDBService packageDBService;
        public static InstalledAppManager installedAppManager;
    }
}
