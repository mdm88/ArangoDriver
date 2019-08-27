using System;

namespace ArangoDriver.dictator
{
	public class InvalidFieldTypeException : Exception
	{
		public InvalidFieldTypeException(string message) : base(message)
		{
		}
	}
}
