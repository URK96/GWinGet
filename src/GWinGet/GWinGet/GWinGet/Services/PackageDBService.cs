using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Data.SQLite.Generic;

using GWinGet.Models;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace GWinGet.Services
{
    public class PackageDBService
    {
        const string APPDATA_PATH = @"C:\Program Files\WindowsApps";

        public List<Package> AvailablePackages { get; private set; }

        public PackageDBService()
        {
            AvailablePackages = new List<Package>();
        }

        public void RefreshList()
        {
            var dbPath = GetNewestDBPath();
            var connectionStr = @$"Data Source={dbPath};Pooling=true;FailIfMissing=false";
            var connectionOption = new DbContextOptionsBuilder<PackageDB>()
                .UseSqlite(connectionStr)
                .Options;

            using var context = new PackageDB(connectionOption);
        }

        private string GetNewestDBPath()
        {
            var dirs = Directory.GetDirectories(APPDATA_PATH, "Microsoft.Winget.Source_*");

            Array.Sort(dirs);

            var newestDirPath = dirs.Last();
            var dbPath = Path.Combine(newestDirPath, "Public", "index.db");

            return dbPath;
        }
    }
}
