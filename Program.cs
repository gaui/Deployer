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
	/// <summary>
	/// Main program
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				Argument arg = null;

				// There have to be some arguments
				if (args == null || args.Length == 0)
				{
					PrintUsage();
					return;
				}

				// Parse and gather arguments
				arg = new Argument(new Arguments(args));

				// Display version info
				if (arg.DisplayVersion)
				{
					PrintVersion();
					return;
				}

				// Parse global settings XML file
				Settings settings = XmlParser.ParseSettings("settings.xml", arg);

				// Parse project XML file
				Project project = XmlParser.ParseProject(arg.ConfigFile, settings, arg);

				// Get the deployment strategy based on the project type
				IDeployStrategy strategy = DeployStrategyFactory.GetStrategyFromType(project.ProjectType);

				foreach (var env in arg.DeploymentEnvironment)
				{
					// Check for confirmation
					if (arg.Confirmation)
					{
						Confirm(arg, settings, project, env);
					}

					// Do backup
					if (arg.Backup)
					{
						Backup(settings, project, env);
					}

					// Do the actual deployment
					var exitCode = strategy.Deploy(arg, settings, project, env);

					// Check if there was an error
					if (exitCode > 0)
					{
						Console.WriteLine("Deployment failed with exit code " + exitCode + ", quitting...");
						Environment.Exit(exitCode);
					}
					// No error, display deployment information
					else
					{
						Console.WriteLine("Build & Deployment successful!");
						PrintInfo(arg, env, settings, project);
					}
				}
			}
			catch (Exception ex)
			{
				PrintText(ex.Message);
				// Exit with an error
				Environment.Exit(1);
			}
		}

		private static void Confirm(Argument arg, Settings settings, Project project, string env)
		{
			string input = "";
			do
			{
				PrintInfo(arg, env, settings, project);
				Console.WriteLine("Continue? (Y/N)");
				input = Console.ReadLine();
			}
			while (input.ToUpper() != "Y" && input.ToUpper() != "N");

			// If user presses "N" (NO)
			if (input.ToUpper() == "N")
				throw new Exception("Deployment aborted");
		}

		private static void Backup(Settings settings, Project project, string env)
		{
			string projectName;

			try
			{
				projectName = project.ProjectName.Substring(0, project.ProjectName.LastIndexOf('.'));
			}
			catch
			{
				projectName = project.ProjectName;
			}

			var backupPath = settings.BackupPath + "\\" + projectName  + "\\" + env + "-" + DateTime.Now.ToString("ddMMyy");

			// Delete directory first
			if (Directory.Exists(backupPath))
				Directory.Delete(backupPath, true);

			// Create the process
			var b = new Process();
			b.StartInfo = new ProcessStartInfo("robocopy.exe") { UseShellExecute = false };
			// I = If destination does not exist and copying more than one file, assumes that destination must be a directory.
			// E = Copies directories and subdirectories, including empty ones.
			// Y = Suppresses prompting to confirm you want to overwrite an existing destination file.
			b.StartInfo.Arguments = "/MIR " + project.Environment[env].DeploymentPath + " " + backupPath;
			b.Start();
			b.WaitForExit();

			// Backup failed, output and continue
			if (b.ExitCode > 1)
				Console.WriteLine("Backup failed with exit code " + b.ExitCode + ", continuing...");
			else
				Console.WriteLine("Backup successful!");
		}

		private static void PrintInfo(Argument arg, string env, Settings settings, Project project)
		{
			Console.WriteLine("Deployment information:");
			Console.WriteLine("-------------------------------------");
			Console.WriteLine("Environment:\t" + env);
			Console.WriteLine("Project path:\t" + project.Environment[env].ProjectPath);
			Console.WriteLine("Project file:\t" + project.ProjectName);
			Console.WriteLine("Project type:\t" + DeployStrategyFactory.GetStrategyName(project.ProjectType));
			Console.WriteLine("Deploy path:\t" + project.Environment[env].DeploymentPath);
			Console.WriteLine("Deploy profile:\t" + project.Environment[env].DeploymentProfile);
			Console.WriteLine("Purge dir:\t" + (arg.PurgeDirectory ? "Yes" : "No"));
			Console.WriteLine("Backup files:\t" + (arg.Backup ? "Yes" : "No"));

			if (arg.Backup)
				Console.WriteLine("Backup path:\t" + settings.BackupPath);

			Console.WriteLine("-------------------------------------");
		}

		private static void PrintText(string text)
		{
			Console.WriteLine("* " + text);
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " -e [ENVIRONMENT] -f [FILE] [OPTIONS]");
			Console.WriteLine("OPTIONS:");
			Console.WriteLine(" -e\tDeployment environment");
			Console.WriteLine(" -f\tProject config file");
			Console.WriteLine(" -p\tPurge directory");
			Console.WriteLine(" -b\tBackup files before deployment");
			Console.WriteLine(" -force\tForce without confirmation");
			Console.WriteLine(" -cd\tWork with projects relative to the current directory");
			Console.WriteLine(" -v\tVersion info");
		}

		private static void PrintVersion()
		{
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string version = fvi.FileVersion;
			Console.WriteLine("Deployer for .NET");
			Console.WriteLine("Version: " + version);
			Console.WriteLine("Author: Gudjon Jonsson");
		}
	}
}
