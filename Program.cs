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

				var backupPath = settings.BackupPath;
				foreach (var env in arg.DeploymentEnvironment)
				{
					// Full project path
					string fullProjectPath = project.Environment[env].ProjectPath + "\\" + project.ProjectName;

					// Setup parameters for MSBuild
					var msbuild_param = string.Format("{0} /p:Configuration={1} /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles={2} /p:publishUrl={3}", fullProjectPath, project.Environment[env].DeploymentProfile, arg.PurgeDirectory ? "True" : "False", project.Environment[env].DeploymentPath);

					// Retrieve backup path (project/method-date)
					settings.BackupPath = backupPath;
					settings.BackupPath = settings.BackupPath + "\\" + project.ProjectName.Substring(0, project.ProjectName.LastIndexOf('.')) + "\\" + env + "-" + DateTime.Now.ToString("ddMMyy");

					// Check for confirmation
					if (arg.Confirmation)
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

					// Do backup
					if (arg.Backup)
					{
						// Delete directory first
						if (Directory.Exists(settings.BackupPath))
							Directory.Delete(settings.BackupPath, true);

						// Create the xcopy process
						var b = new Process();
						b.StartInfo = new ProcessStartInfo("xcopy.exe") { UseShellExecute = false };
						// I = If destination does not exist and copying more than one file, assumes that destination must be a directory.
						// E = Copies directories and subdirectories, including empty ones.
						// Y = Suppresses prompting to confirm you want to overwrite an existing destination file.
						b.StartInfo.Arguments = "/I /E /Y " + project.Environment[env].DeploymentPath + " " + settings.BackupPath;
						b.Start();
						b.WaitForExit();

						// Backup failed, output and continue
						if (b.ExitCode > 0)
							Console.WriteLine("Backup failed with exit code " + b.ExitCode + ", continuing...");
						else
							Console.WriteLine("Backup successful!");
					}

					// Create the MSBuild process
					var p = new Process();
					p.StartInfo = new ProcessStartInfo(settings.MSBuildPath) { UseShellExecute = false };
					p.StartInfo.Arguments = msbuild_param;
					p.Start();
					p.WaitForExit();

					// Check if there was an error
					if (p.ExitCode > 0)
					{
						Console.WriteLine("MSBuild failed with exit code " + p.ExitCode + ", quitting...");
						Environment.Exit(p.ExitCode);
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

		public static void PrintInfo(Argument arg, string env, Settings settings, Project project)
		{
			Console.WriteLine("Deployment information:");
			Console.WriteLine("-------------------------------------");
			Console.WriteLine("Environment:\t" + env);
			Console.WriteLine("Project path:\t" + project.Environment[env].ProjectPath);
			Console.WriteLine("Project file:\t" + project.ProjectName);
			Console.WriteLine("Deploy path:\t" + project.Environment[env].DeploymentPath);
			Console.WriteLine("Deploy profile:\t" + project.Environment[env].DeploymentProfile);
			Console.WriteLine("Purge dir:\t" + (arg.PurgeDirectory ? "Yes" : "No"));
			Console.WriteLine("Backup files:\t" + (arg.Backup ? "Yes" : "No"));

			if (arg.Backup)
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
			Console.WriteLine(" -e\tDeployment environment");
			Console.WriteLine(" -f\tProject config file");
			Console.WriteLine(" -p\tPurge directory");
			Console.WriteLine(" -b\tBackup files before deployment");
			Console.WriteLine(" -force\tForce without confirmation");
			Console.WriteLine(" -cd\tWork with projects relative to the current directory");
			Console.WriteLine(" -v\tVersion info");
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
