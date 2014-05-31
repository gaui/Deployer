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
		public string DeploymentEnvironment { get; set; }
		public string ConfigFile { get; set; }
		public string SettingsFile { get; set; }
		public string PurgeDirectory { get; set; }
		public bool DoBackup { get; set; }
		public bool Confirmation { get; set; }

		public Argument(Arguments arg)
		{
			string homeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// e - Deployment environment (live, staging, test)
			if (arg["e"] == null)
				throw new RequiredArgumentException("Deployment environment");
			else
				this.DeploymentEnvironment = arg["e"];

			// f - Config file
			if (arg["f"] == null)
				throw new RequiredArgumentException("Project config file");
			else
				this.ConfigFile = homeDir + "\\" + arg["f"];

			// s - Settings file
			string settingsFile = "settings.xml";
			if (arg["s"] == null)
				this.SettingsFile = settingsFile;
			else
				this.SettingsFile = arg["s"];

			this.SettingsFile = homeDir + "\\" + this.SettingsFile;

			// p - Purge directory
			if (arg["p"] != null)
				this.PurgeDirectory = arg["p"];

			// b - Do backup (default true)
			bool backup = true;
			if (arg["b"] != null)
			{
				if(Settings.GetFalseValues().Contains(arg["b"]))
					backup = false;
			}
			this.DoBackup = backup;

			// c - Manual confirmation
			bool confirm = true;
			if (arg["c"] != null)
			{
				if (Settings.GetFalseValues().Contains(arg["c"]))
					confirm = false;
			}
			this.Confirmation = confirm;
		}
	}
}
