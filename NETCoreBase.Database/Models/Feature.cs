using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class Feature
    {
        public Feature()
        {
            FeaturePermissions = new HashSet<FeaturePermission>();
            InverseParent = new HashSet<Feature>();
            RoleFeatures = new HashSet<RoleFeature>();
        }

        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int Seq { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual Feature Parent { get; set; }
        public virtual ICollection<FeaturePermission> FeaturePermissions { get; set; }
        public virtual ICollection<Feature> InverseParent { get; set; }
        public virtual ICollection<RoleFeature> RoleFeatures { get; set; }
    }
}
