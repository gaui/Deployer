using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class Argument
	{
		public string DeploymentMethod { get; set; }
		public string ProjectFile { get; set; }
		public string SettingsFile { get; set; }
		public bool PurgeDirectory { get; set; }
		public bool DoBackup { get; set; }
		public string HomeDirectory { get; set; }
		public bool Confirmation { get; set; }

		public Argument(Arguments arg)
		{
			// m - Deployment method (live, staging, test)
			if (arg["m"] == null)
				throw new RequiredArgumentException("Deployment method");
			else
				this.DeploymentMethod = arg["m"];

			// f - Project file
			if (arg["f"] == null)
				throw new RequiredArgumentException("Project file");
			else
				this.ProjectFile = arg["f"];

			// s - Settings file
			string settingsFile = "settings.xml";
			if (arg["s"] != null)
				settingsFile = arg["s"];
			this.SettingsFile = settingsFile;

			// p - Purge directory (default true)
			bool purge = true;
			if (arg["p"] != null)
			{
				if (arg["p"] == "0" || arg["p"] == "false")
					purge = false;
			}
			this.PurgeDirectory = purge;

			// b - Do backup (default true)
			bool backup = true;
			if (arg["b"] != null)
			{
				if (arg["b"] == "0" || arg["b"] == "false")
					backup = false;
			}
			this.DoBackup = backup;

			// h - Home directory
			string homeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (arg["h"] != null)
			{
				if (Directory.Exists(arg["h"]))
					homeDir = arg["h"];
			}
			this.HomeDirectory = homeDir;

			// c - Manual confirmation
			bool confirm = true;
			if (arg["c"] != null)
			{
				if (arg["c"] == "0" || arg["c"] == "false")
					confirm = false;
			}
			this.Confirmation = confirm;
		}
	}
}
