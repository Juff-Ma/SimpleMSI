#region copyright
/*
Config.cs is part of SimpleMSI.
Copyright (C) 2025 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using System.Runtime.Serialization;
using Tomlyn;
using Tomlyn.Model;

namespace SimpleMSI;

public class Config : ITomlMetadataProvider
{
    public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

    [DataMember(IsRequired = true, Name = "general")]
    public GeneralConfig General { get; set; } = new();
    public class GeneralConfig : ITomlMetadataProvider
    {
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

        [DataMember(IsRequired = true, Name = "guid")]
        public string Guid { get; set; } = System.Guid.Empty.ToString();

        [DataMember(IsRequired = true, Name = "name")]
        public string Name { get; set; } = "";

        public const string NameValidationRegex = "^[A-Za-z0-9]+$";

        [DataMember(Name = "platform")]
        public string? Platform { get; set; }

        [DataMember(Name = "version")]
        public string? Version { get; set; }

        [DataMember(Name = "schedule_reboot")]
        public bool? Reboot { get; set; }

        /// <summary>
        /// Sets admin or per-user installation. Possible values are "machine" (admin) and "user".
        /// </summary>
        [DataMember(Name = "install_scope")]
        public string? InstallScope { get; set; }

        /// <summary>
        /// Sets the UI mode of the installer. Possible values are "full", "basic" and "none".
        /// </summary>
        [DataMember(Name = "ui_mode")]
        public string? UiMode { get; set; }

        [DataMember(Name = "out_file")]
        public string? OutFileName { get; set; }
    }

    [DataMember(Name = "meta")]
    public MetadataConfig? Metadata { get; set; }
    public class MetadataConfig : ITomlMetadataProvider
    {
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

        [DataMember(Name = "display_name")]
        public string? DisplayName { get; set; }

        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "manufacturer")]
        public string? Manufacturer { get; set; }

        /// <summary>
        /// License RTF file to be displayed/accepted during installation.
        /// </summary>
        [DataMember(Name = "license_file")]
        public string? LicenseFilePath { get; set; }

        /// <summary>
        /// Image at the top of the installer UI. Must be a 493x58 image file.
        /// </summary>
        [DataMember(Name = "banner_image")]
        public string? BannerImagePath { get; set; }

        /// <summary>
        /// Image at the side of the installer UI. Must be a 493x312 image file.
        /// </summary>
        [DataMember(Name = "dialog_image")]
        public string? DialogImagePath { get; set; }

        /// <summary>
        /// This is the image displayed in the Control Panel/Settings app. Has to be a windows icon file.
        /// </summary>
        [DataMember(Name = "product_icon")]
        public string? ProductIconPath { get; set; }

        [DataMember(Name = "help_url")]
        public string? HelpUrl { get; set; }

        [DataMember(Name = "about_url")]
        public string? AboutUrl { get; set; }

        [DataMember(Name = "update_url")]
        public string? UpdateUrl { get; set; }

        /// <summary>
        /// Removes the Change button from the Control Panel/Settings app. Can still be done using msiexec.
        /// </summary>
        [DataMember(Name = "forbid_modify")]
        public bool? ForbidModify { get; set; }

        /// <summary>
        /// Disables the Repair button from the Control Panel/Settings app. Can still be done using msiexec.
        /// </summary>
        [DataMember(Name = "forbid_repair")]
        public bool? ForbidRepair { get; set; }

        /// <summary>
        /// Removes the Uninstall button from the Control Panel/Settings app. Program can still be uninstalled using the change button or msiexec.
        /// </summary>
        [DataMember(Name = "forbid_uninstall")]
        public bool? ForbidUninstall { get; set; }

        /// <summary>
        /// Registers the software as a System Component, hiding it from the Control Panel/Settings app. Does not impact msiexec usage.
        /// </summary>
        [DataMember(Name = "hide_program_entry")]
        public bool? HideProgramEntry { get; set; }
    }

    [DataMember(Name = "install")]
    public InstallationConfig? Installation { get; set; }
    public class InstallationConfig : ITomlMetadataProvider
    {
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

        /// <summary>
        /// Target installation directory. May contain environment variables like %ProgramFiles% or %LocalAppData%.
        /// </summary>
        [DataMember(Name = "install_dir")]
        public string? Destination { get; set; }

        /// <summary>
        /// Single files to be included in the installation.
        /// </summary>
        [DataMember(Name = "source_files")]
        public List<string> Files { get; } = [];

        /// <summary>
        /// Directories to be included in the installation. Must contain a wildcard in the form of <c>*.*</c> as the filename/>
        /// </summary>
        [DataMember(Name = "source_dirs")]
        public List<string> Dirs { get; } = [];

        [DataMember(Name = "source_dirs_are_recursive")]
        public bool? DirsRecursive { get; set; }

        [DataMember(Name = "env_vars")]
        public List<EnvVarConfig> EnvironmentVariables { get; } = [];
        public class EnvVarConfig : ITomlMetadataProvider
        {
            public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

            [DataMember(IsRequired = true, Name = "name")]
            public string Name { get; set; } = "";

            /// <summary>
            /// Value to set the environment variable to. '@' gets replaced with the installation directory.
            /// </summary>
            [DataMember(IsRequired = true, Name = "value")]
            public string Value { get; set; } = "";

            /// <summary>
            /// May be "all", "prefix" or "suffix". Defaults to "all". "suffix" is commonly used for PATH modifications.
            /// </summary>
            [DataMember(Name = "part")]
            public string? Part { get; set; }
        }

        [DataMember(Name = "shortcuts")]
        public List<ShortcutConfig> Shortcuts { get; } = [];
        public class ShortcutConfig : ITomlMetadataProvider
        {
            public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

            /// <summary>
            /// Files are searched by the end of their filename. So "app.exe" would match "myapp.exe".
            /// </summary>
            [DataMember(IsRequired = true, Name = "target")]
            public string TargetFile { get; set; } = "";
            /// <summary>
            /// May include values like %Desktop%, defaults to the Program Menu.
            /// </summary>
            [DataMember(Name = "location")]
            public string? Location { get; set; }

            /// <summary>
            /// Name of the shortcut. Defaults to the App's name.
            /// </summary>
            [DataMember(Name = "name")]
            public string? Name { get; set; }
        }
    }

    public static Config? FromToml(string toml)
    {
        bool success = Toml.TryToModel(toml, out Config? config, out _);
        return success ? config : null;
    }

    public static async Task<Config?> FromFileAsync(string path, CancellationToken token = default)
    {
        string toml = await File.ReadAllTextAsync(path, token);
        return FromToml(toml);
    }

    public string ToToml()
    {
        return Toml.FromModel(this);
    }
}

