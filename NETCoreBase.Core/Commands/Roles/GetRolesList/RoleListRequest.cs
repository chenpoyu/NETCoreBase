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

namespace NETCoreBase.Core.Commands.Roles
{
    public class RoleListRequest : QueryOption, IRequest<PageResult<RoleListResponse>>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色狀態
        /// </summary>
        public string Status { get; set; }

        public Expression<Func<Role, bool>> GetExpression()
        {
            Expression<Func<Role, bool>> expression = null;

            var name = Name?.Trim();
            var status = Status?.Trim();
            if (Id.HasValue)
            {
                expression = expression.And(x => x.Id == Id.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                expression = expression.And(x => x.Name.Contains(name) || x.NormalizedName.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                expression = expression.And(x => x.Status == status);
            }
            // 查詢未刪除的角色
            expression = expression.And(x => x.Status != "D");

            return expression;
        }
    }
}
