Deployer
========

A deployer CLI that uses MSBuild to build and deploy .NET projects or robocopy to deploy HTML/JavaScript projects.

## Supports

* Simple XML configuration
* Multiple deployment environments per project
* Multiple deployment strategies (for .NET and for HTML/JavaScript apps)
* Web.config transformations
* Backing up files before deployment (using xcopy)
* Purging directories before deployment
* Confirmation (displaying info) before deployment

## Syntax

**Usage:** `Deployer.exe -e [ENVIRONMENT] -f [FILE] [OPTIONS]`

**OPTIONS:**

	-e     Deployment environment
	-f     Project config file
	-p     Purge directory
	-b     Backup files before deployment
	-force Run without confirmation
	-cd    Work with projects relative to the current directory
	-v     Version info

## Configuration

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

**Example *Project.xml* file for a .NET project:**

    <?xml version="1.0" encoding="utf-8"?>
	<project>
		<name>Project.csproj</name>
		<path>Relative\Path\To\Project</path>
        <type>net</type>
		<environment>
			<live>
				<path>\\server\C$\inetpub\wwwroot\LIVE</path>
				<profile>Release</profile>
			</live>
			<staging>
				<path>\\server\C$\inetpub\wwwroot\STAGING</path>
				<profile>Staging</profile>
			</staging>
			<test>
				<path>\\server\C$\inetpub\wwwroot\TEST</path>
				<profile>Debug</profile>
			</test>
		</environment>
	</project>

**Example *Project.xml* file for a pure HTML/JavaScript project:**

    <?xml version="1.0" encoding="utf-8"?>
	<project>
		<name>Project</name>
		<path>Relative\Path\To\Project</path>
        <type>html</type>
		<environment>
			<live>
				<path>\\server\C$\inetpub\wwwroot\LIVE</path>
			</live>
			<staging>
				<path>\\server\C$\inetpub\wwwroot\STAGING</path>
			</staging>
			<test>
				<path>\\server\C$\inetpub\wwwroot\TEST</path>
			</test>
		</environment>
	</project>

## Usage

    Deployer.exe -e live -f Project.xml -p -b

**Would:**

* Read `Project.xml` configuration file
* Display confirmation with information about process
* Backup `\\server\C$\inetpub\wwwroot\LIVE` to `C:\Backup\Project\live-010614`
* Build `Project.csproj` (if type is `net`)
* Deploy `live` environment to `\\server\C$\inetpub\wwwroot\LIVE` along with:
 * Purging directory
 * Transforming Web.config using `Release` profile (if type is `net`)

### Confirmation example:

	Deployment information:
	-------------------------------------
	Environment:    live
	Project path:   C:\Projects\LIVE\Relative\Path\To\Project
	Project file:   Project.csproj
	Deploy path:    \\server\C$\inetpub\wwwroot\LIVE
	Deploy profile: Debug
	Purge dir:      Yes
	Backup files:   Yes
	Backup path:    C:\Backup\Project\live-010614
	-------------------------------------
	Continue? (Y/N)
	
### Multiple deployment environments
It's also possible to deploy to multiple environments in one go. You simply specify environments seperated by a comma `,`

**Example:**

    Deployer.exe -e live,staging -f Project.xml -p -b

### Multiple deployment strategies
It's also possible to deploy using different deployment strategies. Supported strategies are .NET/C# (`net`) and pure HTML/JavaScript apps (`html`).

The .NET strategy uses MSBuild. The HTML strategy uses robocopy.

### Integration with CI like TeamCity
It's also possible to integrate with TeamCity. Just create a simple *Command Line* build configuration. You will need to keep the executable and configuration files on the build server. Deployer will automatically return the same status code MSBuild returns, so if the build and deployment fails, the process will be marked as **failed**.

![TeamCity](http://i.imgur.com/Lu6OTvH.png "TeamCity Command Line build configuration")

*Introducing `-cd` so TeamCity can call Deployer from any directory and always work with projects relative to the current directory (`./`). This means you can both use Deployer in its own directory, specifying base paths for projects in `settings.xml` and with TeamCity's `work/` directory.*

*Also when integrating with TeamCity (or any other CI), remember always to use `-force` argument, to run the process without confirmation*.

## Changelog

###v1.1

* Two different deployment strategies (.NET or HTML/JS apps)
* Replaced xcopy with robocopy, both for backup and JS/HTML deployment
* Code clean up (structure & namespaces)

###v1.0.2

* Exits appropriately (status code 1) when there is an exception thrown (so TeamCity will fail when there is an error).

###v1.0.1

* Confirmation is now default. Use `-force` to run without confirmation.
* Settings file `-s` is no longer an option.
* Added option `-cd` to run project file from current directory (`./`)
