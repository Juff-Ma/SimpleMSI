#region copyright
/*
CommonCommand.cs is part of SimpleMSI.
Copyright (C) 2025 Julian Rossbach

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

    [CliOption(Description = "Version of the app", Required = false)]
    public Version? Version { get; set; }

    [CliOption(Description = "Platform the installer should run on", Required = false,
        AllowedValues = [
            "x86",
            "x64",
            "arm32",
            "arm64"
        ])]
    public string? Platform { get; set; }

    [CliOption(Description = "Output file path", Required = false,
        ValidationRules = CliValidationRules.LegalPath, Alias = "o")]
    public string? OutputFile { get; set; }

    [CliOption(Description = "Source directories to include files from, can be provided multiple times",
        Required = false, Name = "dir")]
    public List<string> SourceDirectories { get; set; } = [];

    [CliOption(Description = "Print extended output")]
    public bool Verbose { get; set; }
}