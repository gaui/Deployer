using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class XmlNodeException : Exception
	{
		public XmlNodeException(string message)
			: base(message + " not available!")
		{
		}
	}
}
