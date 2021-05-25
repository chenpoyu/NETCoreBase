using System;

namespace NETCoreBase.Common.Exceptions
{
    public class CustomValidationException : Exception
    {
		public object ModelErrors { get; set; }

		public CustomValidationException(string message) : base(message)
		{ }

		public CustomValidationException(object error, string message = "") : base(message)
		{
			ModelErrors = error;
		}
    }
}