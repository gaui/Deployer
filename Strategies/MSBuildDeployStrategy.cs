using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Deployer.Models;

namespace Deployer.Strategies
{
	public class MSBuildDeployStrategy : IDeployStrategy
	{
		public string Name { get { return "C# / .NET"; } }

		public int Deploy(Argument arg, Settings settings, Project project, string env)
		{
			// Full project path
			string fullProjectPath = project.Environment[env].ProjectPath + "\\" + project.ProjectName;

			// Setup parameters for MSBuild
			var msbuild_param = string.Format("{0} /p:Configuration={1} /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles={2} /p:publishUrl={3}", fullProjectPath, project.Environment[env].DeploymentProfile, arg.PurgeDirectory ? "True" : "False", project.Environment[env].DeploymentPath);

			// Create the MSBuild process
			var p = new Process();
			p.StartInfo = new ProcessStartInfo(settings.MSBuildPath) { UseShellExecute = false };
			p.StartInfo.Arguments = msbuild_param;
			p.Start();
			p.WaitForExit();

			return p.ExitCode;
		}
	}
}
