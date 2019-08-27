using System;

namespace ArangoDriver.External.dictator
{
	public class InvalidFieldTypeException : Exception
	{
		public InvalidFieldTypeException(string message) : base(message)
		{
		}
	}
}
