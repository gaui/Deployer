using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CommandLine.Utility;

namespace Deployer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				Argument arg = null;

				try
				{
					// There have to be some arguments
					if(args == null || args.Length == 0)
						throw new ArgumentException(GetUsage());

					// Parse and gather arguments
					arg = new Argument(new Arguments(args));
				}
				catch(ArgumentException ex)
				{
					throw ex;
				}

				// Set project filename
				arg.ConfigFile = arg.HomeDirectory + "\\" + arg.ConfigFile;

				// Parse global settings XML file
				Settings settings = XmlParser.ParseSettings(arg.SettingsFile, arg);

				// Parse project XML file
				Project project = XmlParser.ParseProject(arg.ConfigFile, settings, arg);

				// Full project path
				string fullProjectPath = project.ProjectPath + "\\" + project.ProjectName;

				// Setup parameters for MSBuild
				var msbuild_param = string.Format("{0} /p:Configuration={1} /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles={2} /p:publishUrl={3}", fullProjectPath, project.DeploymentProfile, settings.DeleteFiles ? "True" : "False", project.DeploymentPath);

				// Retrieve backup path (project/method-date)
				settings.BackupPath = settings.BackupPath + "\\" + project.ProjectName.Substring(0, project.ProjectName.LastIndexOf('.')) + "\\" + arg.DeploymentMethod + "-" + DateTime.Now.ToString("ddMMyy");

				// Ask user if he wants to continue
				string input = "";
				do
				{
					Console.WriteLine("Deployment information:");
					Console.WriteLine("-------------------------------------");
					Console.WriteLine("Deploy:\t\t" + arg.DeploymentMethod);
					Console.WriteLine("Project:\t" + fullProjectPath);
					Console.WriteLine("Deploy profile:\t" + project.DeploymentProfile);
					Console.WriteLine("Delete files:\t" + (settings.DeleteFiles ? "Yes" : "No"));
					Console.WriteLine("Deploy path:\t" + project.DeploymentPath);
					Console.WriteLine("Backup:\t\t" + (arg.DoBackup ? "Yes" : "No"));

					if(arg.DoBackup)
						Console.WriteLine("Backup path:\t" + settings.BackupPath);

					Console.WriteLine("-------------------------------------");
					Console.WriteLine("Continue? (Y/N)");
					input = Console.ReadLine();
				}
				while(input.ToUpper() != "Y" && input.ToUpper() != "N");

				// If user presses "N" (NO)
				if (input.ToUpper() == "N")
					throw new Exception("Deployment aborted");

				// Else we continue...

				// Do backup
				if (arg.DoBackup)
				{
					// Delete directory first
					if(Directory.Exists(settings.BackupPath))
						Directory.Delete(settings.BackupPath, true);

					// Create the xcopy process
					var b = new Process();
					b.StartInfo = new ProcessStartInfo("xcopy.exe") { UseShellExecute = false };
					// I = If destination does not exist and copying more than one file, assumes that destination must be a directory.
					// E = Copies directories and subdirectories, including empty ones.
					// Y = Suppresses prompting to confirm you want to overwrite an existing destination file.
					b.StartInfo.Arguments = "/I /E /Y " + project.DeploymentPath + " " + settings.BackupPath;
					b.Start();
					b.WaitForExit();
				}

				// Create the MSBuild process
				var p = new Process();
				p.StartInfo = new ProcessStartInfo(settings.MSBuildPath) { UseShellExecute = false };
				p.StartInfo.Arguments = msbuild_param;
				p.Start();
				p.WaitForExit();
			}
			catch (Exception ex)
			{
				PrintText(ex.Message);
			}
		}

		public static void PrintText(string text)
		{
			Console.WriteLine(" * " + text);
		}

		public static string GetUsage()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("WRONG USAGE!");
			sb.AppendLine("Usage comes here!");

			return sb.ToString();
		}
	}
}
