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

            //default to x64
            null => Platform.x64,
            _ => null
        };
    }

    /// <summary>
    /// Gets <see cref="Config.GeneralConfig.Version"/> as a <see cref="Version"/> instance.
    /// </summary>
    /// <returns>The parsed <see cref="Version"/> or <c>null</c> if invalid</returns>
    public static Version? GetVersion(this Config.GeneralConfig config)
    {
        bool result = Version.TryParse(config.Version, out var version);
        return result ? version : new(1, 0);
    }

    /// <summary>
    /// Determines the installation scope based on the configuration settings.
    /// </summary>
    /// <returns>An <see cref="InstallScope"/> value representing the installation scope.  Returns <see
    /// cref="InstallScope.perMachine"/> if the scope is set to "machine" or is <see langword="null"/>.  Returns <see
    /// cref="InstallScope.perUser"/> if the scope is set to "user".  Returns <see langword="null"/> for any other
    /// value.</returns>
    public static InstallScope? GetInstallScope(this Config.GeneralConfig config)
    {
        return config.InstallScope switch
        {
            "machine" => InstallScope.perMachine,
            "user" => InstallScope.perUser,

            //default to perMachine
            null => InstallScope.perMachine,
            _ => null
        };
    }

    /// <summary>
    /// Determines the Wix UI mode based on the specified configuration.
    /// </summary>
    /// <remarks>This method maps the UI mode string from the configuration to a predefined Wix UI mode. If
    /// the UI mode is not specified or is invalid, it defaults to <see cref="WUI.WixUI_ProgressOnly"/>.</remarks>
    /// <returns>A <see cref="WUI"/> value representing the corresponding Wix UI mode: <see cref="WUI.WixUI_ProgressOnly"/> for
    /// "none" or a null value, <see cref="WUI.WixUI_Minimal"/> for "basic", <see cref="WUI.WixUI_InstallDir"/> for
    /// "full", or <c>null</c> if the UI mode is unrecognized.</returns>
    public static WUI? GetWixUIMode(this Config.GeneralConfig config)
    {
        return config.UiMode switch
        {
            "none" => WUI.WixUI_ProgressOnly,
            "basic" => WUI.WixUI_Minimal,
            "full" => WUI.WixUI_InstallDir,
            
            //default to none
            null => WUI.WixUI_ProgressOnly,
            _ => null
        };
    }
}
