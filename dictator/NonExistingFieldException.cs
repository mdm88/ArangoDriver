using System;

namespace ArangoDriver.dictator
{
	public class NonExistingFieldException : Exception
	{
		public NonExistingFieldException(string message) : base(message)
		{
		}
	}
}
