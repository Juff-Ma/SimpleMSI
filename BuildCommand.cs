#region copyright
/*
BuildCommand.cs is part of SimpleMSI.
Copyright (C) 2025 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using DotMake.CommandLine;
using System.Diagnostics;

namespace SimpleMSI;

[CliCommand(Description = "Build MSI from config file")]
internal class BuildCommand : CommonCommand
{

    [CliOption(Description = "Path to configuration file", Required = false,
        Name = "config", Alias = "c",
        ValidationRules = CliValidationRules.LegalFileName | CliValidationRules.ExistingFile)]
    public string? ConfigFile { get; set; }

    [CliOption(Description = "EXE/DLL File to grab Version from", Required = false,
        Name = "grab-version-from-file",
        ValidationRules = CliValidationRules.LegalPath | CliValidationRules.ExistingFile)]
    public string? VersionFile { get; set; }

    [CliOption(Description = "Name of the code signing certificate to use", Required = false)]
    public string? CertificateName { get; set; }

    [CliOption(Description = "Password of the code signing certificate", Required = false)]
    public string? CertificatePassword { get; set; }

    public override async Task<int> RunAsync(CliContext cliContext)
    {
        if (PrintLogo)
        {
            cliContext.ShowLogo();
            await cliContext.Output.WriteLineAsync();
        }

        var print = cliContext.ToPrintContext(Verbose);

        print.VerboseLine("Loading config file...");

        var configPath = ConfigFile;

        if (string.IsNullOrEmpty(configPath))
        {
            var files =
                Directory.GetFiles(Environment.CurrentDirectory, $"*.v{SimpleMsiCli.AssemblyMajor}.msi.toml");

            if (files.Length > 1)
            {
                print.ErrLine("Multiple config files found, please specify one using --config");
                return ExitCodes.InvalidArguments;
            }

            if (files.Length <= 0)
            {
                print.VerboseLine("No versioned config file found, trying without...");
                files = Directory.GetFiles(Environment.CurrentDirectory, "*.msi.toml");
            }

            if (files.Length > 1)
            {
                print.ErrLine(
                    $"Multiple config files found, please specify one using --config. Also consider using versioned config files (config.v{SimpleMsiCli.AssemblyMajor}.msi.toml).");
                return ExitCodes.InvalidArguments;
            }

            if (files.Length <= 0)
            {
                print.ErrLine("No config file found, please specify one using --config");
                return ExitCodes.FileNotFound;
            }
            configPath = files[0];

            print.OutLine($"No config file specified, using '{configPath}'");
        }

        string configText;

        try
        {
            configText = await File.ReadAllTextAsync(configPath);
        }
        catch (Exception ex)
        {
            print.ErrLine($"Error: Failed to read config file '{configPath}': {ex.Message}");
            return ExitCodes.FileNotFound;
        }

        var config = Config.FromToml(configText);

        if (config is null)
        {
            print.ErrLine("Error in config file. Please check the names and values of all fields.");
            return ExitCodes.InvalidConfig;
        }

        print.VerboseLine("Adjusting config...");

        if (VersionFile is not null && Version is not null)
        {
            print.ErrLine("Error: You may not specify both a version and version file");
            return ExitCodes.InvalidArguments;
        }

        if (Version is not null)
        {
            config.General.Version = Version.ToString();
        }

        if (VersionFile is not null)
        {
            var attributes = FileVersionInfo.GetVersionInfo(VersionFile);
            config.General.Version = attributes.FileVersion ?? throw new InvalidDataException("File does not contain valid version information");
        }

        if (Platform is not null)
        {
            config.General.Platform = Platform;
        }

        if (SourceDirectories.Count > 0)
        {
            config.Installation ??= new();
            config.Installation.Dirs.AddRange(SourceDirectories);
        }

        if (SourceFiles.Count > 0)
        {
            config.Installation ??= new();
            config.Installation.Files.AddRange(SourceDirectories);
        }

        if (OutputFile is { } file &&
            string.IsNullOrWhiteSpace(Path.GetFileName(file)))
        {
            print.ErrLine($"Error: Output path '{file}' must contain a file name");
            return ExitCodes.InvalidArguments;
        }

        config.General.OutFileName = OutputFile ?? config.General.OutFileName;

        if (CertificateName is not null)
        {
            config.Installation ??= new();
            config.Installation.Signing ??= new();
            config.Installation.Signing.CertificateName = CertificateName;
        }

        if (CertificatePassword is not null)
        {
            config.Installation ??= new();
            config.Installation.Signing ??= new();
            config.Installation.Signing.Password = CertificatePassword;
        }

        print.VerboseLine("Configuring WiX...");
        MsiEngine.ApplyGlobalConfiguration(Verbose);

        print.OutLine("Configuring MSI package...");
        MsiEngine engine = new(print);

        try
        {
            engine.ConfigureMsi(config);
        }
        catch (ArgumentException e)
        {
            print.ErrLine($"Config error while configuring MSI: {e.Message}");
            return ExitCodes.InvalidConfig;
        }
        catch (FileNotFoundException e)
        {
            print.ErrLine($"Error: Could not find file '{e.FileName}': {e.Message}");
            return ExitCodes.FileNotFound;
        }

        print.OutLine("Building MSI package (this may take a while)...");

        engine.BuildMsi();

        return ExitCodes.Success;
    }
}