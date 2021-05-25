using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreBase.Core.Commands.Features
{
    public class FeatureByUserIdResponse
    {
        /// <summary>
        /// 功能Id
        /// </summary>
        [DisplayName("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 父功能Id
        /// </summary>
        [DisplayName("父功能Id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 順序
        /// </summary>
        [DisplayName("順序")]
        public int Seq { get; set; }

        /// <summary>
        /// 功能名稱
        /// </summary>
        [DisplayName("功能名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 功能路徑
        /// </summary>
        [DisplayName("功能路徑")]
        public string Path { get; set; }

        /// <summary>
        /// 狀態（A：啟用、D：刪除、U：不顯示）
        /// </summary>
        [DisplayName("狀態")]
        public string Status { get; set; }

        public List<FeatureByIdResponse> children { get; set; }
    }
}
