using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
