using System;
using System.Collections.Generic;

#nullable disable

namespace NETCoreBase.Database.Models
{
    public partial class UserLogin
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Status { get; set; }
        public Guid? UserId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
