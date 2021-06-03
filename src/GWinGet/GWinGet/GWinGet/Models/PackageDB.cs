using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWinGet.Models
{
    public class PackageDB : DbContext
    {
        public DbSet<Metadata> Metadatas { get; set; }
        public DbSet<Manifest> Manifests { get; set; }
        public DbSet<Ids> Ids { get; set; }
        public DbSet<Names> Names { get; set; }
        public DbSet<Versions> Versions { get; set; }
        public DbSet<Publishers> Publishers { get; set; }
        public DbSet<PublishersMap> PublishersMaps { get; set; }

        public PackageDB(DbContextOptions<PackageDB> options) : base(options) { }
    }

    [Table("metadata")]
    public class Metadata
    {
        [Key]
        [Column("name")]
        public string Name { get; set; }

        [Column("value")]
        public string Value { get; set; }
    }

    [Table("manifest")]
    public class Manifest
    {
        [Key]
        [Column("rowid")]
        public int RowId { get; set; }

        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public long NameId { get; set; }
        [Column("moniker")]
        public long MonikerId { get; set; }
        [Column("version")]
        public long VersionId { get; set; }
        [Column("channel")]
        public long ChannelId { get; set; }
        [Column("pathpart")]
        public long PathPartId { get; set; }
        [Column("hash")]
        public string Hash { get; set; }
    }

    [Table("ids")]
    public class Ids
    {
        [Key]
        [Column("rowid")]
        public int RowId { get; set; }

        [Column("id")]
        public string Id { get; set; }
    }

    [Table("names")]
    public class Names
    {
        [Key]
        [Column("rowid")]
        public int RowId { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }

    [Table("versions")]
    public class Versions
    {
        [Key]
        [Column("rowid")]
        public int RowId { get; set; }

        [Column("version")]
        public string Version { get; set; }
    }

    [Table("norm_publishers")]
    public class Publishers
    {
        [Key]
        [Column("rowid")]
        public int RowId { get; set; }

        [Column("norm_publisher")]
        public string Publisher { get; set; }
    }

    [Table("norm_publishers_map")]
    public class PublishersMap
    {
        [Key]
        [Column("manifest")]
        public long ManifestId { get; set; }

        [Column("norm_publisher")]
        public long PublisherId { get; set; }
    }
}
