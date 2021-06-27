using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Management.Deployment;
using Windows.ApplicationModel;

using GPackage = GWinGet.Models.Package;

namespace GWinGet.Services
{
    public class InstalledAppManager
    {
        public List<GPackage> InstalledPackages { get; private set; }

        public InstalledAppManager()
        {
            InstalledPackages = new List<GPackage>();
        }

        public void RefreshList()
        {
            InstalledPackages.Clear();

            FindFromPackageManager();
            FindFromRegistry();
        }

        private void FindFromPackageManager()
        {
            PackageManager pManager = new();
            List<Package> pList = pManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();

            foreach (Package packageInfo in pList)
            {
                PackageId packageId = packageInfo.Id;
                string packageVersion = $"{packageId.Version.Major}.{packageId.Version.Minor}.{packageId.Version.Revision}.{packageId.Version.Build}";

                GPackage package = new()
                {
                    Name = packageInfo.DisplayName,
                    PackageId = packageId.Name,
                    Publisher = packageInfo.PublisherDisplayName
                };

                package.versions.Add(packageVersion);

                InstalledPackages.Add(package);
            }
        }

        private void FindFromRegistry()
        {
            const string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using RegistryKey localMachineKey = Registry.LocalMachine.OpenSubKey(uninstallKey);
            using RegistryKey currentUserKey = Registry.CurrentUser.OpenSubKey(uninstallKey);

            AddFromRegSubKey(localMachineKey);
            AddFromRegSubKey(currentUserKey);
        }

        private void AddFromRegSubKey(RegistryKey key)
        {
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using RegistryKey subKey = key.OpenSubKey(subKeyName);

                object displayName = subKey.GetValue("DisplayName");
                object displayVersion = subKey.GetValue("DisplayVersion");
                object publisher = subKey.GetValue("Publisher");
                object packageId = subKeyName;

                if (displayName == null)
                {
                    continue;
                }

                GPackage package = new()
                {
                    Name = displayName as string,
                    Publisher = (publisher == null) ? string.Empty : publisher as string,
                    PackageId = subKeyName
                };

                package.versions.Add((displayVersion == null) ? "Unknown" : displayVersion as string);

                InstalledPackages.Add(package);
            }
        }
    }
}
