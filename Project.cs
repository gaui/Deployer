using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class Project
	{
		public string ProjectName { get; set; }
		public string ProjectPath { get; set; }
		public string DeploymentPath { get; set; }
		public string DeploymentProfile { get; set; }
	}
}
