using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETCoreBase.Common;
using MediatR;
using NETCoreBase.Database.Models;
using NETCoreBase.Common.Utilities.Linq;
using System.Linq.Expressions;

namespace NETCoreBase.Core.Commands.Users
{
    public class UserListRequest : QueryOption, IRequest<PageResult<UserListResponse>>
    {
        /// <summary>
        /// 使用者Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者狀態
        /// </summary>
        public string Status { get; set; }

        public Expression<Func<User, bool>> GetExpression()
        {
            Expression<Func<User, bool>> expression = null;

            var userName = UserName?.Trim();
            var status = Status?.Trim();
            if (Id.HasValue)
            {
                expression = expression.And(x => x.Id == Id.Value);
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                expression = expression.And(x => x.UserName.Contains(userName));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                expression = expression.And(x => x.Status == status);
            }
            // 查詢未刪除的使用者
            expression = expression.And(x => x.Status != "D");

            return expression;
        }
    }
}
