# SimpleMSI
[release-version]: https://img.shields.io/github/v/release/Juff-Ma/SimpleMSI?sort=date&display_name=tag&style=flat-square&logo=github
[download]: https://img.shields.io/badge/nuget-download-004880?style=flat-square&logo=nuget
[actions]: https://img.shields.io/github/actions/workflow/status/Juff-Ma/SimpleMSI/push.yml?branch=main&style=flat-square&logo=githubactions&logoColor=FFFFFF
[license]: https://img.shields.io/github/license/Juff-Ma/SimpleMSI?style=flat-square&logo=opensourceinitiative&logoColor=FFFFFF&color=green



[![][release-version]](https://github.com/Juff-Ma/SimpleMSI/releases/latest)
[![][download]](https://www.nuget.org/packages/SimpleMSI/)
[![][actions]](https://github.com/Juff-Ma/SimpleMSI/actions)
[![][license]](https://github.com/Juff-Ma/SimpleMSI/blob/main/LICENSE.txt)

## Introduction

SimpleMSI is a humble command line utility to create MSI packages the easy way.

Almost all other tools for creating MSI packages either:

1. Are paid and proprietary software
2. Are really complicated and/or have unreadable configuration formats
3. Require you to know a language like C# or JS to script the installer

SimpleMSI throws all that out the window and changes it to the following:

1. Free and open source
2. Simple command line interface with sensible defaults
3. No scripting required, just a plain TOML file

Of course, SimpleMSI does not have all the features of more complex tools like WiX or Advanced Installer.
However most projects only need an installer to do one thing: to install software and that SimpleMSI can do.

In fact, SimpleMSI is based on WiX and uses a subset of its features so you won't need to worry about compatibility or security while not having to bother about its complexity and internals.

Most MSI creation tools map to the complexity of MSI packages and try to expose as much functionality as possible.
SimpleMSI takes a step back and focuses on the most common use cases and makes those as easy as possible to achieve.

## Quick Start
### Prerequisites
SimpleMSI only runs on Windows and requires the WiX CLI (version 5+) to be installed.

You can download WiX from [GitHub](https://github.com/wixtoolset/wix/releases/latest) or install via [dotnet as a tool](https://docs.firegiant.com/wix/using-wix/#command-line-net-tool).
### Installation
As with WiX, you can install SimpleMSI as a dotnet tool:
```powershell
dotnet tool install -g SimpleMSI
```

Alternatively, if you don't have dotnet installed, you can download the latest version from the [releases page](https://github.com/Juff-Ma/SimpleMSI/releases/latest).

### Running SimpleMSI
To create an MSI package, you need to create a configuration file in TOML format.
SimpleMSI can help you with that using the `init` command:
```powershell
simplemsi init YourApp -en your-main.exe -sd build-dir\*.*
```
And just like that you should have a file called `YourApp.v1.msi.toml` with a basic configuration to get you started.

You can then edit the file to customize it to your needs or you can just run SimpleMSI to create the MSI package:
```powershell
simplemsi build
```

What next? Nothing. That's it! You just created your first MSI package with SimpleMSI.

## Command Line Reference
If you just run `simplemsi` without any arguments, you will see the following help message:

```txt
SimpleMSI Windows Installer creation tool.

Usage:
  SimpleMSI [command] [options]

Options:
  -nl, --nologo   Do not display logo and copyright [default: False]
  -?, -h, --help  Show help and usage information
  -v, --version   Show version information

Commands:
  i, init   Initialize new config file
  b, build  Build MSI from config file
```
As you can see, SimpleMSI has two main commands: `init` and `build`.
### Init
The `init` command creates a new configuration file for your application. Specifying the application name is required, everything else is optional.

However, you will most likely want to at least specify the source directory or files to include in the installer using the `-sd` or `-sf` options as well as the main exe name using `-en`
since without them the build process could fail.

Of course you can also adjust those options in the created config file later on.
```txt
init: Initialize new config file

Usage:
  SimpleMSI init <app-name> [options]

Arguments:
  <app-name>  Application identifying name [required]

Options:
  -g, --guid <guid>                         Application identifying GUID
  -en, --executable-name <executable-name>  Executable name used for default Shortcut
  -v, --version <version>                   Version of the app
  -p, --platform <arm32|arm64|x64|x86>      Platform the installer should run on
  -o, --output-file <output-file>           Output file path
  -sd, --dir <dir>                          Source directories to include files from, can be provided multiple times
  -sf, --file <file>                        Source files include, can be provided multiple times
  --verbose                                 Print extended output [default: False]
  -nl, --nologo                             Do not display logo and copyright [default: False]
  -?, -h, --help                            Show help and usage information
```

### Build
The `build` is what actually does the magic of creating the MSI package from the configuration file.

Options you specify on the command line will override those in the config file with the exception of source directories and files which will be merged with those in the config file.

You also have the options of grabbing the version from an existing EXE or DLL file using the `-vf` option, which is not accessible via the config file.
```txt
build: Build MSI from config file

Usage:
  SimpleMSI build [options]

Options:
  -c, --config <config>                                   Path to configuration file
  -vf, --grab-version-from-file <grab-version-from-file>  EXE/DLL File to grab Version from
  -v, --version <version>                                 Version of the app
  -p, --platform <arm32|arm64|x64|x86>                    Platform the installer should run on
  -o, --output-file <output-file>                         Output file path
  -sd, --dir <dir>                                        Source directories to include files from, can be provided multiple times
  -sf, --file <file>                                      Source files include, can be provided multiple times
  --verbose                                               Print extended output [default: False]
  -nl, --nologo                                           Do not display logo and copyright [default: False]
  -?, -h, --help                                          Show help and usage information
```

## Configuration File Reference

The configuration file is written in TOML format and contains all the settings for creating the MSI package.

Here is an example configuration file with all available options and comments to explain them:
```toml
# General configuration
# This is the part that configures the main properties of your MSI installer.
[general]

# This is the GUID that uniquely identifies your app. This needs to be present and should not be changed once your app is published.
# You can generate a new GUID using various online tools or development environments.
# If it is changed after publishing, it may cause issues with updates and installations.
# It will be automatically generated for you when you first initiate your config and most people will not need to change it.
guid = "a9bb4baa-e7b4-47e1-b5c5-13ec0490a1ab"

# This is the name that identifies your app.
# It will be used in various places internally, such as the installation folder.
# The name may not contain spaces or special characters.
# Note: if you define meta.display_name, this will almost never be used.
name = "ExampleApp"

# The platform your app is built for. Options are "x86", "x64", "arm32" and "arm64".
# The default is "x64".
platform = "x64"

# Version number with at least two and max four parts separated by dots.
# It may not contain any additional labels like "beta" or "alpha".
# The last part (Revision) is ONLY METADATA to MSI and will be ignored by Windows Installer.
# If a Revision is specified, it will be ignored by upgrade detection as well and the MSI will install as a separate app.
# If you need to upgrade revisions use allow_same_version_upgrades but note that this will also allow downgrades (e.g. 1.0.0.15 -> 1.0.0.3).
# The default version is "1.0.0".
version = "1.0.0"

# Whether to allow downgrades during installation.
# If this is set to true, users will be able to install an older version of the app over a newer version.
# This is generally not recommended but you may need it in some scenarios.
# The default is false.
allow_downgrades = false

# Whether to allow upgrades to the same version during installation.
# If true, users will be able to reinstall the same version of the app.
# Generally this is not required since MSI supports repairing installations, but you may need it in some scenarios.
# The default is false.
allow_same_version_upgrades = false

# The installation scope of your app. Options are "user" and "machine".
# If you choose "user", the app will be installed only for the current user only.
# If you choose "machine", the app will be installed for all users on the machine and will require admin rights.
# The default is "machine".
install_scope = "machine"

# The UI mode of the installer. Options are "full", "basic" and "none".
# "full" provides the complete Windows Installer UI with license agreement, installation folder selection, progress bar, etc.
# "basic" provides a simplified UI with just a license agreement and installation progress.
# "none" provides just a small window with a progress bar.
# The default is "none".
ui_mode = "basic"

# The output file name of the generated MSI installer.
# The default is "<name>-<version>-<platform>.msi".
out_file = "ExampleAppInstaller.msi"

# Metadata information
# This section contains additional information about your app that will be displayed in various places.
# Data here is optional but recommended as it will be used to augment some the existing information.
[meta]

# The display name of your app.
# This is the name that will be shown to users during installation and in the Programs and Features list.
# This will also override general.name in most places visible to the user.
display_name = "Example Application"

# A short description of your app.
description = "An example application packaged as an MSI installer."

# The publisher of your app.
author = "Example Author"

# The path to the license file for your app.
# This file will be shown to users during installation if the UI mode is "full" or "basic".
# This should be a valid RTF file.
license_file = ".\\LICENSE.rtf"

# The path to the banner image for your app to replace the default one.
# This image will be shown at the top of the installer window if the UI mode is "full" or "basic".
# PNG format is recommended. The image should be 493x58 pixels for best results.
banner_image = ".\\banner.png"

# The path to the dialog image for your app to replace the default one.
# This image will be shown on the left side of the installer window if the UI mode is "full" or "basic".
# PNG format is recommended. The image should be 493x312 pixels for best results.
dialog_image = ".\\dialog.png"

# The path to the icon file for your app.
# This icon will be used to represent your app, for example in the Programs and Features list.
# ICO format is required.
product_icon = ".\\icon.ico"

# Website where users can get help with your app.
help_url = "https://example.com/help"

# Website with more information about your app.
about_url = "https://example.com/about"

# Website where users can check for updates to your app.
update_url = "https://example.com/update"

# This removes the "Change" button from the Programs and Features entry for your app.
# Note: This does not prevent users from modifying the installation via other means like the msiexec command line.
# The default is false.
forbid_modify = false

# This removes the "Repair" button from the Programs and Features entry for your app.
# Note: This does not prevent users from repairing the installation via other means like the msiexec command line.
# The default is false.
forbid_repair = false

# This removes the "Uninstall" button from the Programs and Features entry for your app.
# Note: This does not prevent users from uninstalling the app via other means like the msiexec command line.
# The default is false.
forbid_uninstall = false

# This will mark your app as a system component and thereby hide it from the Programs and Features list.
# This is generally not recommended unless you have a specific reason to do so.
# This does not impact the ability to modify the app via other means like the msiexec command line.
# The default is false.
hide_program_entry = false

# Installation information
# This is where you define what files to install, environment variables to set, shortcuts to create, etc.
[install]

# The installation directory for your app.
# You can use common variables like %ProgramFiles% or %LocalAppData%.
# Default is "%ProgramFiles%\\<name>" for machine installs and "%LocalAppData%\\Programs\\<name>" for user installs.
install_dir = "%ProgramFiles%\\ExampleApp"

# Installation source files.
# These files will be directly packaged.
source_files = [".\\readme.txt", ".\\config\\default.cfg"]

# Installation source directories.
# These are the directories where your application files to be packaged are taken from.
# This must contain a wildcard to include all files automatically.
source_dirs = [".\\app\\*.*"]

# Whether to include files in subdirectories of the source_dirs.
# The default is true.
source_dirs_are_recursive = true

# Environment variables to set during installation.
[[install.env_vars]]

# The name of the environment variable to set.
name = "PATH"

# The value to set the environment variable to.
# "@" gets expanded to the installation directory of the app.
value = "@"

# Where to add the value.
# "all" replaces the variable, "prefix" adds the value to the start, "suffix" adds the value to the end.
# Use "suffix" when modifying PATH to ensure other important paths are not overridden.
# The default is "all".
part = "suffix"

# since you may want to add multiple environment variables, through the magic of TOML table arrays, you can define another one here.
[[install.env_vars]]
name = "EXAMPLEAPP_INSTALLED"
value = "1"

# This defines shortcuts to create during installation.
# As with environment variables, you can define multiple shortcuts using multiple entries.
[[install.shortcuts]]

# The target executable for the shortcut.
# This matches all files regardless of subdirectory, so be specific to avoid conflicts.
# Also, this matches back to front, so "app.exe" would match "exampleapp.exe".
# In general, it's best to use the main executable of your app here.
target = "exampleapp.exe"

# This is where the shortcut will be created.
# You can use common variables like %Desktop%.
# The default is to place it in the Start Menu.
# Generally, you should always have a shortcut in the start menu, so one where location is omitted.
location = "%Desktop%"

# The name as displayed for the shortcut.
name = "Start ExampleApp"
```

## Appendix
### Building from source
SimpleMSI is built using .NET 10.0. To build SimpleMSI from source, clone the repository and build it like any other .NET project.

When publishing SimpleMSI you need to ensure that you specify the runtime identifier for Windows, e.g. `win-x64` or `win-arm64`.

Packing SimpleMSI as a dotnet tool is a different story. SimpleMSI uses a few tricks to be packages a tool, so you need to make sure `-p:RuntimeIdentifiers=""` is set.
Another such hack is used for the windows-specific dependencies, since dotnet tools are supposed to be cross-platform, this doesn't change anything in the build process but should be kept in the back of your mind.

### How it works

SimpleMSI uses [WixSharp](https://github.com/oleg-shilo/wixsharp) under the hood.
WixSharp is a wonderful tool for creating MSI packages in a scripted way but it still suffers from the WiX complexity since it to tries to expose all functionality in a way that maps closely to the original.

It configures the MSI by generating a WixSharp project on the fly based on the provided configuration file and command line arguments.

Afterwards it instructs WixSharp to build a wxs (WiX source) file and invokes the WiX CLI tools to compile and link the wxs into an MSI package.
