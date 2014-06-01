using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployer
{
	/// <summary>
	/// Deployment and backup settings
	/// </summary>
	public class Settings
	{
		public string HomeDirectory { get; set; }
		public string MSBuildPath { get; set; }
		public string BackupPath { get; set; }
		public Dictionary<string, string> ProjectBase { get; set; }

		public Settings()
		{
			this.ProjectBase = new Dictionary<string, string>();
		}

		public static List<string> GetFalseValues()
		{
			return new List<string> { "0", "false" };
		}
	}
}
