using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Users
{
    public class DeleteUserRequest : IRequest<Unit>
    {
        /// <summary>
        /// 使用者Id
        /// </summary>
        public List<Guid> Id { get; set; }
    }
}
