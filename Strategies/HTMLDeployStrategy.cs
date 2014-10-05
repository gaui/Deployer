using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Deployer.Models;

namespace Deployer.Strategies
{
	public class HTMLDeployStrategy : IDeployStrategy
	{
		public string Name { get { return "HTML / JavaScript"; } }

		public int Deploy(Argument arg, Settings settings, Project project, string env)
		{
			// Full project path
			string fullProjectPath = project.Environment[env].ProjectPath;

			// Create the process
			var b = new Process();
			b.StartInfo = new ProcessStartInfo("robocopy.exe") { UseShellExecute = false };
			// I = If destination does not exist and copying more than one file, assumes that destination must be a directory.
			// E = Copies directories and subdirectories, including empty ones.
			// Y = Suppresses prompting to confirm you want to overwrite an existing destination file.
			b.StartInfo.Arguments = "/MIR " + fullProjectPath + " " + project.Environment[env].DeploymentPath;
			b.Start();
			b.WaitForExit();

			if (b.ExitCode > 1)
				return 1;
			else
				return 0;
		}
	}
}
