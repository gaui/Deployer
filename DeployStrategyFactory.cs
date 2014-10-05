using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class DeployStrategyFactory
	{
		public static IDeployStrategy GetStrategyFromType(string type)
		{
			IDeployStrategy strategy;

			// Check the project type
			switch (type)
			{
				case "html":
					strategy = new HTMLDeployStrategy();
					break;
				case "net":
					strategy = new MSBuildDeployStrategy();
					break;
				default:
					strategy = new MSBuildDeployStrategy();
					break;
			}

			return strategy;
		}

		public static string GetStrategyName(string type)
		{
			IDeployStrategy strategy = GetStrategyFromType(type);

			return strategy.Name;
		}
	}
}
