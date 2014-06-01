Deployer
========

A .NET deployer CLI that uses MSBuild to build and deploy .NET projects.

Supports
--
- Easy XML configuration
- Multiple deployment environments per project
- Web.config transformations
- Backing up files before deployment (using xcopy)
- Purging directories before deployment
- Confirmation (displaying info) before deployment

Syntax
--
**Usage:** `Deployer.exe -e [ENVIRONMENT] -f [FILE] [OPTIONS]`
OPTIONS:

	-e     Deployment environment
	-f     Project config file
	-s     Settings file
	-p     Purge directory
	-b     Backup files before deployment
	-c     Confirm before deployment
	-v     Version info

Sample
--

**Example *settings.xml* file:**

	<?xml version="1.0" encoding="utf-8"?>
	<settings>
		<msbuild>C:\Windows\Microsoft.NET\Framework\v4.0.30319</msbuild>
		<backup>C:\Backup</backup>
		<projects>
			<live>C:\Projects\LIVE</live>
			<staging>C:\Projects\STAGING</staging>
			<test>C:\Projects\TEST</test>
		</projects>
	</settings>

**Example *Project.xml* file:**

    <?xml version="1.0" encoding="utf-8"?>
	<project>
		<name>Project.csproj</name>
		<path>Relative\Path\To\Project</path>
		<environment>
			<live>
				<path>\\server\C$\inetpub\wwwroot\LIVE</path>
				<profile>Live</profile>
			</live>
			<staging>
				<path>\\server\C$\inetpub\wwwroot\STAGING</path>
				<profile>Staging</profile>
			</staging>
			<test>
				<path>\\server\C$\inetpub\wwwroot\TEST</path>
				<profile>Test</profile>
			</test>
		</environment>
	</project>
