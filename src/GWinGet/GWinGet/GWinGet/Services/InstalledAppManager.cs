using GWinGet.Models;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Management.Deployment;

namespace GWinGet.Services
{
    public class InstalledAppManager
    {
        public List<Package> InstalledPackages { get; private set; }

        public InstalledAppManager()
        {
            InstalledPackages = new List<Package>();
        }

        public void RefreshList()
        {
            InstalledPackages.Clear();

            FindFromPackageManager();
            FindFromRegistry();
        }

        private void FindFromPackageManager()
        {
            var pManager = new PackageManager();
            var pList = pManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();

            foreach (var packageInfo in pList)
            {
                var packageId = packageInfo.Id;
                var packageVersion = $"{packageId.Version.Major}.{packageId.Version.Minor}.{packageId.Version.Revision}.{packageId.Version.Build}";

                var package = new Package
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

            using var localMachineKey = Registry.LocalMachine.OpenSubKey(uninstallKey);
            using var currentUserKey = Registry.CurrentUser.OpenSubKey(uninstallKey);

            AddFromRegSubKey(localMachineKey);
            AddFromRegSubKey(currentUserKey);
        }
        
        private void AddFromRegSubKey(RegistryKey key)
        {
            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);

                var displayName = subKey.GetValue("DisplayName");
                var displayVersion = subKey.GetValue("DisplayVersion");
                var publisher = subKey.GetValue("Publisher");
                var packageId = subKeyName;

                if (displayName == null)
                {
                    continue;
                }

                var package = new Package
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
