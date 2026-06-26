using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class Banner : NETCoreBase.Database.ISoftDeletable
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Enable { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public long Version { get; set; }
        // Banner uses Enable as status; expose Status for ISoftDeletable compatibility
        string NETCoreBase.Database.ISoftDeletable.Status => Enable;
    }
}
