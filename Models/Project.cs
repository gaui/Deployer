using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer.Models
{
	public class ProjectEnvironment
	{
		public string ProjectPath { get; set; }
		public string DeploymentPath { get; set; }
		public string DeploymentProfile { get; set; }
	}

	/// <summary>
	/// Deployment and project settings
	/// </summary>
	public class Project
	{
		public string ProjectName { get; set; }
		public string ProjectType { get; set; }
		public Dictionary<string, ProjectEnvironment> Environment { get; set; }

		public Project()
		{
			this.Environment = new Dictionary<string, ProjectEnvironment>();
		}
	}
}
