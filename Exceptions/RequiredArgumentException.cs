using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer.Exceptions
{
	/// <summary>
	/// Exception for required arguments
	/// </summary>
	public class RequiredArgumentException : Exception
	{
		public RequiredArgumentException(string message)
			: base(message + " is required!")
		{
		}
	}
}
