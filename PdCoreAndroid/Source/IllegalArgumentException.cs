using System;

namespace PdCore
{
	public class IllegalArgumentException : Exception
	{
		public IllegalArgumentException () : base() {}
		public IllegalArgumentException (String error) : base(error) {}
	}
}

