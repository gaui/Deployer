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
				Settings settings = XmlParser.ParseSettings(arg.SettingsFile, arg);

				// Parse project XML file
				Project project = XmlParser.ParseProject(arg.ConfigFile, settings, arg);

				// Full project path
				string fullProjectPath = project.ProjectPath + "\\" + project.ProjectName;

				// Setup parameters for MSBuild
				var msbuild_param = string.Format("{0} /p:Configuration={1} /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles={2} /p:publishUrl={3}", fullProjectPath, project.DeploymentProfile, settings.PurgeDirectory ? "True" : "False", project.DeploymentPath);

				// Retrieve backup path (project/method-date)
				settings.BackupPath = settings.BackupPath + "\\" + project.ProjectName.Substring(0, project.ProjectName.LastIndexOf('.')) + "\\" + arg.DeploymentEnvironment + "-" + DateTime.Now.ToString("ddMMyy");

				// Check for confirmation
				if (arg.Confirmation)
				{
					string input = "";
					do
					{
						PrintInfo(arg, settings, project);
						Console.WriteLine("Continue? (Y/N)");
						input = Console.ReadLine();
					}
					while (input.ToUpper() != "Y" && input.ToUpper() != "N");

					// If user presses "N" (NO)
					if (input.ToUpper() == "N")
						throw new Exception("Deployment aborted");
				}

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

		public static void PrintInfo(Argument arg, Settings settings, Project project)
		{
			Console.WriteLine("Deployment information:");
			Console.WriteLine("-------------------------------------");
			Console.WriteLine("Environment:\t" + arg.DeploymentEnvironment);
			Console.WriteLine("Project path:\t" + project.ProjectPath);
			Console.WriteLine("Project file:\t" + project.ProjectName);
			Console.WriteLine("Deploy path:\t" + project.DeploymentPath);
			Console.WriteLine("Deploy profile:\t" + project.DeploymentProfile);
			Console.WriteLine("Purge dir:\t" + (settings.PurgeDirectory ? "Yes" : "No"));
			Console.WriteLine("Backup files:\t" + (arg.DoBackup ? "Yes" : "No"));

			if (arg.DoBackup)
				Console.WriteLine("Backup path:\t" + settings.BackupPath);

			Console.WriteLine("-------------------------------------");
		}

		public static void PrintText(string text)
		{
			Console.WriteLine("* " + text);
		}

		public static void PrintUsage()
		{
			Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " -e [ENVIRONMENT] -f [FILE] [OPTIONS]");
			Console.WriteLine("OPTIONS:");
			Console.WriteLine("  -e\tDeployment environment");
			Console.WriteLine("  -f\tProject config file");
			Console.WriteLine("  -s\tSettings file");
			Console.WriteLine("  -p\tPurge directory");
			Console.WriteLine("  -b\tBackup files before deployment");
			Console.WriteLine("  -c\tConfirm before deployment");
			Console.WriteLine("  -v\tVersion info");
		}

		public static void PrintVersion()
		{
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string version = fvi.FileVersion;
			Console.WriteLine("Deployer for .NET");
			Console.WriteLine("Version: " + version);
			Console.WriteLine("Author: Gudjon Jonsson");
		}
	}
}
