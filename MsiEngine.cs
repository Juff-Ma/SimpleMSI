#region copyright
/*
    MsiEngine.cs is part of SimpleMSI.
    Copyright (C) 2025 Julian Rossbach

    This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion

using WixSharp;

namespace SimpleMSI;

/// <summary>
/// The main class for configuring and building MSIs.
/// </summary>
public class MsiEngine
{
    public void BuildMsi(Config config, PrintContext print = default)
    {
        print.VerboseLine("Configuring msi...");
        Project msi = new(config.General.Name)
        {
            GUID = config.General.GetGuid() ??
                       throw new ArgumentException("Main GUID is not valid", nameof(config)),
            Platform = config.General.GetWixPlatform() ??
                           throw new ArgumentException("Platform is not valid", nameof(config))
        };
    }
}
