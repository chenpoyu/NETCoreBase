using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class Role : NETCoreBase.Database.ISoftDeletable
    {
        public Role()
        {
            RoleFeatures = new HashSet<RoleFeature>();
            UserRoles = new HashSet<UserRole>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public long Version { get; set; }

        public virtual ICollection<RoleFeature> RoleFeatures { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
