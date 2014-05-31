using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Deployer
{
	public static class XmlParser
	{
		public static Settings ParseSettings(string filename, Argument arg)
		{
			Settings settings = new Settings();

			XmlDocument doc_settings = new XmlDocument();
			doc_settings.Load(filename);

			// Retrieve settings node
			XmlNode node_settings = doc_settings.DocumentElement.SelectSingleNode("/settings");
			if (node_settings == null)
				throw new XmlNodeException("Settings info not available!");

			// Retrieve MSBuild node
			XmlNode node_msbuild = node_settings["msbuild"];
			if (node_msbuild == null)
				throw new XmlNodeException("MSBuild info not available!");

			// Set MSBuild path
			settings.MSBuildPath = node_msbuild.InnerText + "\\msbuild.exe";
			if (!File.Exists(settings.MSBuildPath))
				throw new FileNotFoundException("MSBuild file not found!");

			// Delete files before deployment
			XmlNode node_purge = node_settings["purge"];
			if (node_purge != null && (node_purge.InnerText.ToLower() == "true" || node_purge.InnerText.ToLower() == "false"))
			{
				// p - Purge directory (default true)
				bool purge = true;
				if (arg.PurgeDirectory != null)
				{
					if (arg.PurgeDirectory == "0" || arg.PurgeDirectory == "false")
						purge = false;
				}
				else
				{
					purge = Convert.ToBoolean(node_purge.InnerText.ToLower());
				}

				settings.PurgeDirectory = purge;
			}

			// Delete files before deployment
			XmlNode node_projectsBasePath = node_settings["projects"];
			if (node_projectsBasePath == null)
				throw new XmlNodeException("Projects base path not available!");

			settings.ProjectBase = node_projectsBasePath[arg.DeploymentMethod].InnerText;

			// Retrieve backup path
			XmlNode node_backup = node_settings["backup"];
			if (node_backup != null && arg.DoBackup)
				settings.BackupPath = node_settings["backup"].InnerText;

			return settings;
		}

		public static Project ParseProject(string filename, Settings settings, Argument arg)
		{
			Project project = new Project();

			// Retrieve project settings
			XmlDocument doc_project = new XmlDocument();
			doc_project.Load(filename);

			// Retrieve project node
			XmlNode node_project = doc_project.DocumentElement.SelectSingleNode("/project");
			if (node_project == null)
				throw new XmlNodeException("Project info");

			// Retrieve project name node
			XmlNode node_projectName = node_project["name"];
			if (node_projectName == null)
				throw new XmlNodeException("Project name info");

			project.ProjectName = node_project["name"].InnerText;

			// Retrieve project path node
			XmlNode node_projectPath = node_project["path"];
			if (node_projectPath == null)
				throw new XmlNodeException("Project path info");

			project.ProjectPath = settings.ProjectBase + "\\" + node_projectPath.InnerText;

			if (!Directory.Exists(project.ProjectPath))
				throw new FileNotFoundException("Project path doesn't exist!");

			// Retrieve deployment node
			XmlNode node_deployment = node_project["deployment"][arg.DeploymentMethod];
			if (node_deployment == null)
				throw new XmlNodeException("Deployment info");

			// Retrieve deployment path node
			XmlNode node_deploymentPath = node_deployment["path"];
			if (node_deploymentPath == null)
				throw new XmlNodeException("Deployment path info");

			project.DeploymentPath = node_deploymentPath.InnerText;

			// Retrieve deployment profile node
			XmlNode node_deploymentProfile = node_deployment["profile"];
			if (node_deploymentProfile == null)
				throw new XmlNodeException("Deployment profile info");

			project.DeploymentProfile = node_deploymentProfile.InnerText;

			return project;
		}
	}
}
