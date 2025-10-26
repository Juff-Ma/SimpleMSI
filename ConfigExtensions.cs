#region copyright
/*
    ConfigExtensions.cs is part of SimpleMSI.
    Copyright (C) 2025 Julian Rossbach

    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using WixSharp;

namespace SimpleMSI;

/// <summary>
/// Non-public extension methods for <see cref="Config"/>.
/// </summary>
internal static class ConfigExtensions
{
    /// <summary>
    /// Gets <see cref="Config.GeneralConfig.Guid"/> from the config as a <see cref="Guid"/> instance.
    /// </summary>
    /// <returns>The parsed <see cref="Guid"/>, or <c>null</c> if invalid</returns>
    public static Guid? GetGuid(this Config.GeneralConfig config)
    {
        bool result = Guid.TryParse(config.Guid, out var guid);
        return result ? guid : null;
    }

    /// <summary>
    /// Gets the WixSharp <see cref="Platform"/> from the <see cref="Config.GeneralConfig.Platform"/> attribute.
    /// </summary>
    /// <returns>The parsed <see cref="Platform"/> or <c>null</c> if invalid</returns>
    public static Platform? GetWixPlatform(this Config.GeneralConfig config)
    {
        return config.Platform switch
        {
            "x86" => Platform.x86,
            "x64" => Platform.x64,
            "arm32" => Platform.arm,
            "arm64" => Platform.arm64,
            _ => null
        };
    }
}
