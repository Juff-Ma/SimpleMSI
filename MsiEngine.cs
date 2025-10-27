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
using File = System.IO.File;

namespace SimpleMSI;

/// <summary>
/// The main class for configuring and building MSIs.
/// </summary>
public class MsiEngine(PrintContext print = default)
{
    private Project? _msi;

    public void ConfigureMsi(Config config)
    {
        if (config.General.Name.Contains(' ') || string.IsNullOrWhiteSpace(config.General.Name))
        {
            throw new ArgumentException("Name may not be empty or contain whitespaces", nameof(config));
        }
        
        print.VerboseLine("Configuring msi...");
        _msi = new(config.Metadata?.DisplayName ?? config.General.Name)
        {
            GUID = config.General.GetGuid() ??
                       throw new ArgumentException("Main GUID is not valid", nameof(config)),
            Platform = config.General.GetWixPlatform() ??
                           throw new ArgumentException("Platform is not valid", nameof(config)),
            Version = config.General.GetVersion() ??
                          throw new ArgumentException("Version is not valid", nameof(config)),
            Scope = config.General.GetInstallScope() ?? 
                    throw new ArgumentException("Install Scope is not valid", nameof(config)),
            UI = config.General.GetWixUIMode() ?? 
                 throw new ArgumentException("UI Mode is not valid", nameof(config)),

            Description = config.Metadata?.Description ?? string.Empty,
            MajorUpgradeStrategy = MajorUpgradeStrategy.Default,
        };

        if (config.General.Reboot == true)
        {
            _msi.ScheduleReboot = new();
        }

        if (config.Metadata?.LicenseFilePath is var license && license is not null && !Path.Exists(license))
        {
            throw new FileNotFoundException("License file not found", license);
        }
        _msi.LicenceFile = license ?? string.Empty;

        if (config.Metadata?.BannerImagePath is var banner && banner is not null && !Path.Exists(banner))
        {
            throw new FileNotFoundException("Banner image not found", banner);
        }
        _msi.BannerImage = banner ?? string.Empty;

        if (config.Metadata?.DialogImagePath is var dialog && dialog is not null && !Path.Exists(dialog))
        {
            throw new FileNotFoundException("Dialog image not found", dialog);
        }
        _msi.BackgroundImage = dialog ?? string.Empty;

        _msi.ControlPanelInfo = new()
        {
            InstallLocation = "[INSTALLDIR]",
            Manufacturer = config.Metadata?.Manufacturer ?? config.General.Name,
            Comments = config.Metadata?.Description,

            HelpLink = config.Metadata?.HelpUrl,
            UrlInfoAbout = config.Metadata?.AboutUrl,
            UrlUpdateInfo = config.Metadata?.UpdateUrl,

            NoModify = config.Metadata?.ForbidModify,
            NoRepair = config.Metadata?.ForbidRepair,
            NoRemove = config.Metadata?.ForbidUninstall,
            SystemComponent = config.Metadata?.HideProgramEntry,
        };

        if (config.Metadata?.ProductIconPath is var icon && icon is not null && !Path.Exists(icon))
        {
            throw new FileNotFoundException("Product icon not found", icon);
        }
        _msi.ControlPanelInfo.ProductIcon = icon;

        var outFile = config.General.OutFileName;
        if (string.IsNullOrWhiteSpace(outFile))
        {
            var filename =
                $"{config.General.Name}-{config.General.Version ?? "1.0.0"}-{config.General.Platform ?? "x64"}";
            var currentDir = Environment.CurrentDirectory;

            outFile = Path.Combine(currentDir, filename);
            print.VerboseLine($"No output file specified, using '{outFile}'");
        }

        if (outFile.EndsWith(".msi"))
        {
            outFile = outFile[..^4]; // Remove .msi extension since WixSharp adds it automatically
        }

        if (Path.GetDirectoryName(outFile) is var dir && !string.IsNullOrEmpty(dir))
        {
            _msi.OutDir = dir;
        }

        if (Path.GetFileName(outFile) is var file && string.IsNullOrEmpty(file))
        {
            throw new ArgumentException("Filename is not valid", nameof(config));
        }

        _msi.OutFileName = file;
    }
}
