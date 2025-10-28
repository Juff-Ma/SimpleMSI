#region copyright
/*
Program.cs is part of SimpleMSI.
Copyright (C) 2025 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using DotMake.CommandLine;
using SimpleMSI;

return await Cli.RunAsync<SimpleMsiCli>(args, new()
{
    EnableDefaultExceptionHandler = true,
    Theme = CliTheme.NoColor,
    EnablePosixBundling = true,
    Output = Console.Out,
    Error = Console.Error
});

namespace SimpleMSI
{
    /// <summary>
    /// Root command for SimpleMSI CLI.
    /// </summary>
    [CliCommand(Description = "SimpleMSI Windows Installer creation tool.",
        Children = [
            typeof(InitCommand),
            typeof(BuildCommand)
        ])]
    internal class SimpleMsiCli : ICliRunWithContextAndReturn
    {
        [CliOption(Name = "nologo", Recursive = true, Description = "Do not display logo and copyright")]
        public bool NoLogo { get; set; }

        public int Run(CliContext cliContext)
        {
            if (!NoLogo)
            {
                cliContext.ShowLogo();
                cliContext.WriteLine("\n");
            }

            cliContext.ShowHelp();
            return ExitCodes.InvalidArguments;
        }
    }
}