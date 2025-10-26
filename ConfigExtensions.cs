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

internal static class ConfigExtensions
{
    public static Guid? GetGuid(this Config.GeneralConfig config)
    {
        bool result = Guid.TryParse(config.Guid, out var guid);
        return result ? guid : null;
    }

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
