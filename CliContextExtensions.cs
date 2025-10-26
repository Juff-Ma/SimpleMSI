#region copyright
/*
	CliContextExtensions.cs is part of SimpleMSI.
	Copyright (C) 2025 Julian Rossbach

	This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

	This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

	You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using DotMake.CommandLine;

namespace SimpleMSI;

internal static class CliContextExtensions
{
    public static readonly string AsciiLogo =
        $"""
         =========================================================
              __ _                 _              __   _____ 
             / _(_)_ __ ___  _ __ | | ___  /\/\  / _\  \_   \
             \ \| | '_ ` _ \| '_ \| |/ _ \/    \ \ \    / /\/
             _\ \ | | | | | | |_) | |  __/ /\/\ \_\ \/\/ /_
             \__/_|_| |_| |_| .__/|_|\___\/    \/\__/\____/
                            |_|                              
         (C) {DateTime.Now.Year} SimpleMSI ======================================
         """;

    public static void ShowLogo(this CliContext ctx)
    {
        ctx.WriteLine(AsciiLogo);
    }

    public static void Write(this CliContext ctx, ReadOnlySpan<char> str)
        => ctx.Output.Write(str);

    public static void WriteLine(this CliContext ctx, ReadOnlySpan<char> str)
        => ctx.Output.WriteLine(str);

    public static Task WriteAsync(this CliContext ctx, ReadOnlyMemory<char> str, CancellationToken token = default)
        => ctx.Output.WriteAsync(str, token);

    public static Task WriteLineAsync(this CliContext ctx, ReadOnlyMemory<char> str, CancellationToken token = default)
        => ctx.Output.WriteLineAsync(str, token);

    public static PrintContext ToPrintContext(this CliContext ctx, bool verbose = false)
    {
        return new PrintContext(verbose, ctx.Output, ctx.Error);
    }
}
