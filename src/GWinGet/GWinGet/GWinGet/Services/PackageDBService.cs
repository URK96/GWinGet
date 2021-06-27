using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GWinGet.Models;

using Microsoft.EntityFrameworkCore;

namespace GWinGet.Services
{
    public class PackageDBService
    {
        const string MAJOR_VER_KEY = "majorVersion";
        const string MINOR_VER_KEY = "minorVersion";
        const string LAST_WRITE_KEY = "lastwritetime";
        const string APPDATA_PATH = @"C:\Program Files\WindowsApps";

        public List<Package> AvailablePackages { get; private set; }

        public string DBVersion => $"{dbMajorVersion}.{dbMinorVersion} ({dbLastWriteTime})";

        public string dbMajorVersion;
        public string dbMinorVersion;
        public string dbLastWriteTime;

        public PackageDBService()
        {
            AvailablePackages = new List<Package>();
        }

        public void RefreshList()
        {
            string dbPath = GetNewestDBPath();
            string connectionStr = @$"Data Source={dbPath}";
            var connectionOption = new DbContextOptionsBuilder<PackageDB>()
                .UseSqlite(connectionStr)
                .Options;

            using PackageDB context = new(connectionOption);

            AvailablePackages.Clear();

            try
            {
                foreach (Manifest item in context.Manifests)
                {
                    string packageId = context.Ids.Find((int)item.Id).Id;
                    int index = AvailablePackages.FindIndex(x => x.PackageId.Equals(packageId));

                    string refVersion = context.Versions.Find((int)item.VersionId).Version;

                    if (index < 0)
                    {
                        Package newPackage = new()
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
                        Package package = AvailablePackages[index];

                        package.versions.Add(refVersion);

                        AvailablePackages[index] = package;
                    }
                }

                dbMajorVersion = context.Metadatas.Find(MAJOR_VER_KEY)?.Value;
                dbMinorVersion = context.Metadatas.Find(MINOR_VER_KEY)?.Value;
                dbLastWriteTime = context.Metadatas.Find(LAST_WRITE_KEY)?.Value;
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\Users\URK96\GWinGetError.txt", ex.ToString());
            }
        }

        private string GetNewestDBPath()
        {
            string[] dirs = Directory.GetDirectories(APPDATA_PATH, "Microsoft.Winget.Source_*");

            Array.Sort(dirs);

            string newestDirPath = dirs.Last();
            string dbPath = Path.Combine(newestDirPath, "Public", "index.db");

            return dbPath;
        }
    }
}
