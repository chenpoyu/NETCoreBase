using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class RoleFeature
    {
        public Guid RoleId { get; set; }
        public Guid FeatureId { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Role Role { get; set; }
    }
}
