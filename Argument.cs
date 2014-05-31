﻿using CommandLine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Deployer
{
	/// <summary>
	/// Handles program arguments
	/// </summary>
	public class Argument
	{
		public string DeploymentEnvironment { get; set; }
		public string ConfigFile { get; set; }
		public string SettingsFile { get; set; }
		public bool PurgeDirectory { get; set; }
		public bool DoBackup { get; set; }
		public bool Confirmation { get; set; }
		public bool DisplayVersion { get; set; }

		public Argument(Arguments arg)
		{
			// v - Display version
			if (arg["v"] != null)
			{
				this.DisplayVersion = true;
				return;
			}

			string homeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// e - Deployment environment (live, staging, test)
			if (arg["e"] == null || arg["e"] == "true")
				throw new RequiredArgumentException("Deployment environment");
			else
				this.DeploymentEnvironment = arg["e"];

			// f - Config file
			if (arg["f"] == null || arg["f"] == "true")
				throw new RequiredArgumentException("Project config file");
			else
				this.ConfigFile = homeDir + "\\" + arg["f"];

			// s - Settings file
			string settingsFile = "settings.xml";
			if (arg["s"] == null || arg["s"] == "true")
				this.SettingsFile = settingsFile;
			else
				this.SettingsFile = arg["s"];

			this.SettingsFile = homeDir + "\\" + this.SettingsFile;

			// p - Purge directory (default false)
			if (arg["p"] != null && arg["p"] == "true")
				this.PurgeDirectory = true;
			else
				this.PurgeDirectory = false;

			// b - Do backup (default false)
			bool backup = false;
			if (arg["b"] != null && arg["b"] == "true")
					backup = true;

			this.DoBackup = backup;

			// c - Manual confirmation (default false)
			bool confirm = false;
			if (arg["c"] != null && arg["c"] == "true")
				confirm = true;

			this.Confirmation = confirm;
		}
	}
}
