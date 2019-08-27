using System;

namespace ArangoDriver.dictator
{
	public class InvalidFieldException : Exception
	{
		public InvalidFieldException(string message) : base(message)
		{
		}
	}
}
