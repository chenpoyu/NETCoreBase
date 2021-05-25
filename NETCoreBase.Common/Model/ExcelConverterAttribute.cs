using System;
using System.IO;

namespace NETCoreBase.Common.Model
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcelConverterAttribute : Attribute
    {
        public ExcelConverterAttribute(Type converter)
        {
            if (converter.IsSubclassOf(typeof(IExcelConverter)))
                throw new ArgumentException("converter 必須繼承 ExcelConverter", "converter");
            Converter = converter;
        }

        public Type Converter { get; private set; }
    }

    public interface IExcelConverter
    {
        string Name { get; }
    }

    public abstract class ExcelConverter<T> : IExcelConverter
    {
        public abstract string Name { get; }

        public abstract void Write(T value, Stream output);
    }
}
