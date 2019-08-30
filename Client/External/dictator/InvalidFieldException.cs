using System;

namespace ArangoDriver.External.dictator
{
	public class InvalidFieldException : Exception
	{
		public InvalidFieldException(string message) : base(message)
		{
		}
	}
}
