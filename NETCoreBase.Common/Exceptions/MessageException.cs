using System;

namespace NETCoreBase.Common.Exceptions
{
	public class MessageException : Exception
	{
		public int Status { get; set; } = 500;

		public MessageException(string message) : base(message)
		{ }

		public MessageException(int status, string message = "") : base(message)
		{
			Status = status;
		}
	}
}