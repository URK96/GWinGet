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
            var connectionStr = @$"Data Source={dbPath}";
            var connectionOption = new DbContextOptionsBuilder<PackageDB>()
                .UseSqlite(connectionStr)
                .Options;

            using var context = new PackageDB(connectionOption);

            AvailablePackages.Clear();

            try
            {
                foreach (var item in context.Manifests)
                {
                    var packageId = context.Ids.Find((int)item.Id).Id;
                    var index = AvailablePackages.FindIndex(x => x.PackageId.Equals(packageId));

                    var refVersion = context.Versions.Find((int)item.VersionId).Version;

                    if (index < 0)
                    {
                        var newPackage = new Package
                        {
                            Name = context.Names.Find((int)item.NameId).Name,
                            PackageId = context.Ids.Find((int)item.Id).Id,
                            Publisher = context.Publishers.Find((int)context.PublishersMaps.Find((long)item.RowId).PublisherId).Publisher
                        };

                        newPackage.versions = new List<string>
                        {
                            refVersion
                        };

                        AvailablePackages.Add(newPackage);
                    }
                    else
                    {
                        var package = AvailablePackages[index];

                        package.versions.Add(refVersion);

                        AvailablePackages[index] = package;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\Users\URK96\GWinGetError.txt", ex.ToString());
            }
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
