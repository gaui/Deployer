using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class RequiredArgumentException : Exception
	{
		public RequiredArgumentException(string message)
			: base(message + " is required!")
		{
		}
	}
}
