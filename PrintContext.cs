#region copyright
/*
    PrintContext.cs is part of SimpleMSI.
    Copyright (C) 2025 Julian Rossbach

    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

namespace SimpleMSI;

public struct PrintContext
{
    public PrintContext() {}

    public PrintContext(bool verbose, TextWriter? output, TextWriter error)
    {
        IsVerbose = verbose;
        Output = output;
        Error = error;
    }

    public bool IsVerbose { get; set; }

    public TextWriter? Output { get; set; }
    public TextWriter? Error { get; set; }

    public void OutLine(ReadOnlySpan<char> line)
    {
        Output?.WriteLine(line);
    }

    public void ErrLine(ReadOnlySpan<char> line)
    {
        Error?.WriteLine(line);
    }

    public void VerboseLine(ReadOnlySpan<char> line)
    {
        if (IsVerbose)
        {
            OutLine(line);
        }
    }
}
