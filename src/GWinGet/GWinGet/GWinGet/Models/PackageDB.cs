using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWinGet.Models
{
    public class PackageDB : DbContext
    {
        private readonly string connectionStr;

        public DbSet<manifest> Manifests { get; set; }
        public DbSet<ids> Ids { get; set; }
        public DbSet<names> Names { get; set; }

        public PackageDB(DbContextOptions<PackageDB> options) : base(options) { }
    }

    public class manifest
    {
        public int rowid { get; set; }
        public long id { get; set; }
        public long name { get; set; }
        public long moniker { get; set; }
        public long version { get; set; }
        public long channel { get; set; }
        public long pathpart { get; set; }
    }

    public class ids
    {
        public int rowid { get; set; }
        public string id { get; set; }
    }

    public class names
    {
        public int rowid { get; set; }
        public string name { get; set; }
    }
}
