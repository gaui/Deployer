using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	public class Settings
	{
		public string MSBuildPath { get; set; }
		public bool PurgeDirectory { get; set; }
		public string BackupPath { get; set; }
		public string ProjectBase { get; set; }
	}
}
