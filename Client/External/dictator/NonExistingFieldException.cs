using System;

namespace ArangoDriver.External.dictator
{
	public class NonExistingFieldException : Exception
	{
		public NonExistingFieldException(string message) : base(message)
		{
		}
	}
}
