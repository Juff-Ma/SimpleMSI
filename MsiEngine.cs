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
using WixSharp.CommonTasks;
using WixToolset.Mba.Core;
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

        var scope = config.General.GetInstallScope() ??
                    throw new ArgumentException("Install Scope is not valid", nameof(config));

        print.VerboseLine("Configuring msi files...");

        if (config.Installation?.Destination is var installDestination && string.IsNullOrWhiteSpace(installDestination))
        {
            switch (scope)
            {
                case InstallScope.perMachine: installDestination = "%ProgramFiles%\\"; break;
                case InstallScope.perUser: installDestination = "%LocalAppData%\\"; break;
                default: throw new InvalidOperationException("What the hell just happened"); //should never happen
            }

            if (!string.IsNullOrWhiteSpace(config.Metadata?.Manufacturer))
            {
                installDestination += config.Metadata.Manufacturer + '\\';
            }

            installDestination += config.General.Name;
        }

        var installFiles = (config.Installation?.Files ?? []).Select(f => (new WixSharp.File(f)) as WixEntity);
        var installDirs = (config.Installation?.Dirs ?? []).Select(d =>
        {
            bool recursive = config.Installation?.DirsRecursive != false;
            return (recursive ? new Files(d) as WixEntity : new DirFiles(d));
        });

        InstallDir installDir = new(installDestination, installFiles.Concat(installDirs).ToArray());

        print.VerboseLine("Configuring msi...");
        _msi = new(config.Metadata?.DisplayName ?? config.General.Name, installDir)
        {
            GUID = config.General.GetGuid() ??
                       throw new ArgumentException("Main GUID is not valid", nameof(config)),
            Platform = config.General.GetWixPlatform() ??
                           throw new ArgumentException("Platform is not valid", nameof(config)),
            Version = config.General.GetVersion() ??
                          throw new ArgumentException("Version is not valid", nameof(config)),
            Scope = scope,
            UI = config.General.GetWixUiMode() ?? 
                 throw new ArgumentException("UI Mode is not valid", nameof(config)),

            Description = config.Metadata?.Description ?? string.Empty,
            MajorUpgradeStrategy = MajorUpgradeStrategy.Default,
        };

        _msi.ResolveWildCards();

        foreach (var shortcut in config.Installation?.Shortcuts ?? [])
        {
            var files = _msi.FindFile(f => f.Name.EndsWith(shortcut.TargetFile));
            if (files.Length <= 0)
            {
                throw new FileNotFoundException("Shortcut file not found", shortcut.TargetFile);
            }

            if (files.Length > 1)
            {
                print.OutLine($"Warning: {files.Length} files found for shortcut for {shortcut.TargetFile}, using first hit");
            }

            files[0].AddShortcut(new(shortcut.Name ?? _msi.Name, shortcut.Location ?? "ProgramMenuFolder"));
        }

        foreach (var @var in config.Installation?.EnvironmentVariables ?? [])
        {
            var part = @var.GetEnvVarPart() ?? throw new ArgumentException($"Env var part for variable {@var.Name} not valid", nameof(config));

            _msi.Add(
                new EnvironmentVariable(@var.Name, @var.Value.Replace("@", "[INSTALLDIR]"))
                {
                    Part = part
                }
            );
        }

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

    public void BuildMsi()
    {
        if (_msi is null)
        {
            throw new InvalidOperationException("MSI not configured");
        }
        _msi.BuildMsi();
    }

    public void BuildMsiCmd()
    {
        if (_msi is null)
        {
            throw new InvalidOperationException("MSI not configured");
        }
        _msi.BuildMsiCmd();
    }

    public void BuildWxs()
    {
        if (_msi is null)
        {
            throw new InvalidOperationException("MSI not configured");
        }
        _msi.BuildWxs();
    }
}
