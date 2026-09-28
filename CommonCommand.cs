#region copyright
/*
CommonCommand.cs is part of SimpleMSI.
Copyright (C) 2026 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using DotMake.CommandLine;

namespace SimpleMSI;

internal abstract class CommonCommand : ICliRunAsyncWithContextAndReturn
{
    public abstract Task<int> RunAsync(CliContext cliContext);

    // ReSharper disable once MemberCanBePrivate.Global
    public SimpleMsiCli Root { get; set; } = null!;
    protected bool PrintLogo => !Root.NoLogo;

    [CliOption(Name = "version", Alias = "v", Description = "Version of the app", Required = false)]
    public Version? Version { get; set; }

    [CliOption(Name = "platform", Alias = "p",
        Description = "Platform the installer should run on", Required = false,
        AllowedValues = [
            "x86",
            "x64",
            "arm32",
            "arm64"
        ])]
    public string? Platform { get; set; }

    [CliOption(Name = "output-file", Alias = "o",
        Description = "Output file path", Required = false,
        ValidationRules = CliValidationRules.LegalPath)]
    public string? OutputFile { get; set; }

    [CliOption(Name = "dir", Alias = "d",
        Description = "Source directories to include files from, can be provided multiple times",
        Required = false)]
    public List<string> SourceDirectories { get; set; } = [];

    [CliOption(Name = "file", Alias = "f",
        Description = "Source files include, can be provided multiple times",
        Required = false)]
    public List<string> SourceFiles { get; set; } = [];

    [CliOption(Name = "verbose", Alias = "V",
        Description = "Print extended output")]
    public bool Verbose { get; set; }
}