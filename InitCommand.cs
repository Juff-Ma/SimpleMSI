#region copyright
/*
InitCommand.cs is part of SimpleMSI.
Copyright (C) 2025 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using System.Reflection;
using DotMake.CommandLine;

namespace SimpleMSI;

[CliCommand(Description = "Initialize new config file")]
internal class InitCommand : CommonCommand
{
    private static readonly Assembly assembly = typeof(Marker).Assembly;
    private static readonly int assemblyMajor = assembly.GetName().Version?.Major ?? 1;

    [CliArgument(ValidationPattern = Config.GeneralConfig.NameValidationRegex,
        Description = "Application identifying name")] 
    public string AppName { get; set; } = null!;

    [CliOption(Description = "Application identifying GUID", Required = false)]
    public Guid? Guid { get; set; }

    [CliOption(Description = "Executable name used for default Shortcut", Required = false,
        ValidationRules = CliValidationRules.LegalFileName)]
    public string? ExecutableName { get; set; }

    public override async Task<int> RunAsync(CliContext cliContext)
    {
        if (PrintLogo)
        {
            cliContext.ShowLogo();
            await cliContext.Output.WriteLineAsync();
        }

        var print = cliContext.ToPrintContext(Verbose);

        print.VerboseLine("Loading template config...");

        var resource = assembly.GetManifestResourceStream(typeof(InitCommand), "Template.msi.toml")
                                ?? throw new FileNotFoundException("Embedded template not found");

        using StreamReader reader = new(resource);
        var template = await reader.ReadToEndAsync();

        var config = Config.FromToml(template)
                        ?? throw new InvalidDataException("Failed to parse embedded template");

        print.VerboseLine("Adjusting template...");

        config.General.Name = AppName;
        config.General.Guid = (Guid ?? System.Guid.NewGuid()).ToString();

        if (Version is not null)
        {
            config.General.Version = Version.ToString();
        }

        if (Platform is not null)
        {
            config.General.Platform = Platform;
        }

        if (ExecutableName is not null)
        {
            var shortcut = config.Installation?.Shortcuts[0]!;

            shortcut.TargetFile = ExecutableName;
        }

        if (SourceDirectories.Count > 0)
        {
            config.Installation!.Dirs.Clear();
            config.Installation!.Dirs.AddRange(SourceDirectories);
        }

        return ExitCodes.Success;
    }
}