using System;

namespace NETCoreBase.Common.Model
{ 
    /// <summary>
    /// Excel 排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ExcelAttribute : Attribute
    {
        public ExcelAttribute(int order)
        {
            Order = order;
            Width = -1;
        }

        /// <summary>
        /// 順序
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// 寬度
        /// </summary>
        public double Width { get; init; }

        /// <summary>
        /// 是否加總
        /// </summary>
        public bool HasSum { get; init; } = false;

        /// <summary>
        /// 數字格式
        /// </summary>
        public string NumberFormate { get; init; }
    }
}
