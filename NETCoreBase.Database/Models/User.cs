using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class User : NETCoreBase.Database.ISoftDeletable
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberConfirmed { get; set; }
        public string TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string RequireChangeMima { get; set; }
        public DateTimeOffset? LastChangeMimaDate { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset? Lockout { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public long Version { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
