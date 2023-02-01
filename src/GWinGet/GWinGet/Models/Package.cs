using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWinGet.Models;

public class Package
{
    public string Name { get; set; }
    public string PackageId { get; set; }
    public string Version => versions.First();
    public string Publisher { get; set; }

    public List<string> versions = new List<string>();
}
