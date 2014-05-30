using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Deployer
{
	public class Program
	{
		public static void PrintText(string text)
		{
			Console.WriteLine(" * " + text);
		}

		public static void Main(string[] args)
		{
			try
			{
				if (args == null || args.Length < 2)
					throw new Exception("Wrong usage!\nUSAGE: Deployer.exe live project.xml");

				// Default values
				const string file_settings = "settings.xml";
				const string delete_default = "True";
				bool doBackup = true;

				// Set project filename
				string file_project = args[1];

				// Set deployment method
				string deployment_method = args[0];

				// Backup parameter
				if (args.Length == 3)
				{
					doBackup = Convert.ToBoolean(args[2]);
				}

				// Load global settings file
				XmlDocument doc_settings = new XmlDocument();
				doc_settings.Load(file_settings);

				// Retrieve settings node
				XmlNode node_settings = doc_settings.DocumentElement.SelectSingleNode("/settings");
				if (node_settings == null)
					throw new Exception("Settings info not available!");

				// Retrieve MSBuild node
				XmlNode node_msbuild = node_settings["msbuild"];
				if (node_msbuild == null)
					throw new Exception("MSBuild info not available!");

				// Set MSBuild path
				string msbuild_path = node_msbuild.InnerText + "\\msbuild.exe";
				if (!File.Exists(msbuild_path))
					throw new Exception("MSBuild file not found!");

				// Delete files before deployment
				string delete = delete_default;
				XmlNode node_delete = node_settings["delete"];
				if (node_delete != null && (node_delete.InnerText == "True" || node_delete.InnerText == "False"))
					delete = node_delete.InnerText;
				else
					PrintText("Delete info not available, setting to default " + delete_default);

				// Delete files before deployment
				string project_base;
				XmlNode node_projectsBasePath = node_settings["projects"];
				if (node_projectsBasePath == null)
					throw new Exception("Projects base path not available!");

				project_base = node_projectsBasePath[deployment_method].InnerText;

				// Retrieve project settings
				XmlDocument doc_project = new XmlDocument();
				doc_project.Load(file_project);

				// Retrieve project node
				XmlNode node_project = doc_project.DocumentElement.SelectSingleNode("/project");
				if (node_project == null)
					throw new Exception("Project info not available!");

				// Retrieve project name node
				XmlNode node_projectName = node_project["name"];
				if (node_projectName == null)
					throw new Exception("Project name info not available!");

				string project_name = node_project["name"].InnerText;

				// Retrieve project path node
				XmlNode node_projectPath = node_project["path"];
				if (node_projectPath == null)
					throw new Exception("Project path info not available!");

				string project_path = project_base + "\\" + node_project["path"].InnerText;

				if (!Directory.Exists(project_path))
					throw new Exception("Project path doesn't exist!");

				// Retrieve deployment node
				XmlNode node_deployment = node_project["deployment"][deployment_method];
				if (node_deployment == null)
					throw new Exception("Deployment info not available for \"" + deployment_method + "\"");

				// Retrieve deployment path node
				XmlNode node_deploymentPath = node_deployment["path"];
				if (node_deploymentPath == null)
					throw new Exception("Deployment path info not available!");

				string deployment_path = node_deploymentPath.InnerText;

				// Retrieve deployment profile node
				XmlNode node_deploymentProfile = node_deployment["profile"];
				if (node_deploymentProfile == null)
					throw new Exception("Deployment profile info not available!");

				string deployment_profile = node_deploymentProfile.InnerText;

				// Retrieve backup path
				string backup_path = "";
				XmlNode node_backup = node_settings["backup"];
				if (node_backup != null)
				{
					backup_path = node_backup.InnerText + "\\" + project_name.Substring(0, project_name.LastIndexOf('.')) + "\\" + deployment_method + "-" + DateTime.Now.ToString("ddMMyy");
				}

				// Gather parameters for MSBuild
				project_path = project_path + "\\" + project_name;
				var param = string.Format("{0} /p:Configuration={1} /p:Platform=AnyCPU /t:WebPublish /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles={2} /p:publishUrl={3}", project_path, deployment_profile, delete, deployment_path);

				// Ask user if he wants to continue
				string input = "";
				do
				{
					Console.WriteLine("Deployment information:");
					Console.WriteLine("-------------------------------------");
					Console.WriteLine("Deploy:\t\t" + deployment_method);
					Console.WriteLine("Project:\t" + project_path);
					Console.WriteLine("Deploy profile:\t" + deployment_profile);
					Console.WriteLine("Delete files:\t" + (delete == "True" ? "Yes" : "No"));
					Console.WriteLine("Deploy path:\t" + deployment_path);
					Console.WriteLine("Backup:\t\t" + (doBackup ? "Yes" : "No"));

					if(doBackup)
						Console.WriteLine("Backup path:\t" + backup_path);

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
				if (doBackup)
				{
					// Delete directory first
					if(Directory.Exists(backup_path))
						Directory.Delete(backup_path, true);

					var b = new Process();
					b.StartInfo = new ProcessStartInfo("xcopy.exe") { UseShellExecute = false };
					b.StartInfo.Arguments = "/I /E /Y " + deployment_path + " " + backup_path;
					b.Start();
					b.WaitForExit();
				}

				// Create the process
				var p = new Process();
				p.StartInfo = new ProcessStartInfo(msbuild_path) { UseShellExecute = false };
				p.StartInfo.Arguments = param;
				p.Start();
				p.WaitForExit();
			}
			catch (Exception ex)
			{
				PrintText(ex.Message);
			}
		}
	}
}
