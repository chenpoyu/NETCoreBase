using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace NETCoreBase.Core.Commands.Features
{
    public class CreateFeatureRequest : IRequest<Unit>
    {
        /// <summary>
        /// 父功能Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 順序
        /// </summary>
        public int Seq { get; set; }

        /// <summary>
        /// 功能名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 功能路徑
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 狀態（A：啟用、D：刪除、U：不顯示）
        /// </summary>
        public string Status { get; set; }
    }
}
