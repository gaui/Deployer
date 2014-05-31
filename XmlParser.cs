using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Deployer
{
	/// <summary>
	/// Handles parsing XML files
	/// </summary>
	public static class XmlParser
	{
		public static Settings ParseSettings(string filename, Argument arg)
		{
			Settings settings = new Settings();

			XmlDocument doc_settings = new XmlDocument();
			doc_settings.Load(filename);

			// Set home directory
			settings.HomeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// Retrieve settings node
			XmlNode node_settings = doc_settings.DocumentElement.SelectSingleNode("/settings");
			if (node_settings == null)
				throw new XmlNodeException("Settings info");

			// Retrieve MSBuild node
			XmlNode node_msbuild = node_settings["msbuild"];
			if (node_msbuild == null)
				throw new XmlNodeException("MSBuild info");

			// Set MSBuild path
			settings.MSBuildPath = node_msbuild.InnerText + "\\msbuild.exe";
			if (!File.Exists(settings.MSBuildPath))
				throw new FileNotFoundException("MSBuild file not found!");

			// Delete files before deployment
			XmlNode node_projectsBasePath = node_settings["projects"];
			if (node_projectsBasePath == null)
				throw new XmlNodeException("Projects base path");

			if (node_projectsBasePath[arg.DeploymentEnvironment] == null)
				throw new XmlNodeException("Projects environment path");

			settings.ProjectBase = node_projectsBasePath[arg.DeploymentEnvironment].InnerText;

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

			// Retrieve deployment environment node
			XmlNode node_environment = node_project["environment"][arg.DeploymentEnvironment];
			if (node_environment == null)
				throw new XmlNodeException("Deployment environment");

			// Retrieve deployment environment path node
			XmlNode node_environmentPath = node_environment["path"];
			if (node_environmentPath == null)
				throw new XmlNodeException("Deployment environment path info");

			project.DeploymentPath = node_environmentPath.InnerText;

			// Retrieve deployment environment profile node
			XmlNode node_environmentProfile = node_environment["profile"];
			if (node_environmentProfile == null)
				throw new XmlNodeException("Deployment environment profile info");

			project.DeploymentProfile = node_environmentProfile.InnerText;

			return project;
		}
	}
}
