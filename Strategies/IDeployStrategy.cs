using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployer.Models;

namespace Deployer.Strategies
{
	/// <summary>
	/// Deploy Strategy interface
	/// </summary>
	public interface IDeployStrategy
	{
		string Name { get; }
		/// <summary>
		/// Deploy function
		/// </summary>
		/// <returns>Exit code</returns>
		int Deploy(Argument arg, Settings settings, Project project, string env);
	}
}
