#region copyright
/*
    ExitCodes.cs is part of SimpleMSI.
    Copyright (C) 2025 Julian Rossbach

    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
namespace SimpleMSI;

/// <summary>
/// Exit codes used by the application.
/// </summary>
internal static class ExitCodes
{
    /// <summary>
    /// Unknown error occurred.
    /// </summary>
    public const int UnknownError = -1;

    /// <summary>
    /// Indicates that the operation was successful and everything went smoothly.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Wrong arguments were provided or required arguments are missing.
    /// </summary>
    public const int InvalidArguments = 1;

    /// <summary>
    /// Invalid configuration file or configuration data.
    /// </summary>
    public const int InvalidConfig = 2;

    /// <summary>
    /// File not found.
    /// </summary>
    public const int FileNotFound = 3;
}
