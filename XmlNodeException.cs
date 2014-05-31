using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	/// <summary>
	/// Exception for XML nodes
	/// </summary>
	public class XmlNodeException : Exception
	{
		public XmlNodeException(string message)
			: base(message + " not available!")
		{
		}
	}
}
