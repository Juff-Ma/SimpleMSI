#region copyright
/*
Config.cs is part of SimpleMSI.
Copyright (C) 2026 Julian Rossbach

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
#endregion
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Serialization;

namespace SimpleMSI;

public class Config
{
    [JsonIgnore] 
    private TomlMetadataStore? TomlMetadataStore { get; set; }
    
    [JsonPropertyName("general")]
    [JsonRequired]
    public GeneralConfig General { get; set; } = new();
    public class GeneralConfig
    {
        [JsonPropertyName("guid")]
        [JsonRequired]
        public string Guid { get; set; } = System.Guid.Empty.ToString();

        [JsonPropertyName("name")]
        [JsonRequired]
        public string Name { get; set; } = "";

        [JsonIgnore]
        public const string NameValidationRegex = "^[A-Za-z0-9\\.\\-_\\+]+$";

        [JsonPropertyName("platform")]
        public string? Platform { get; set; }

        /// <summary>
        /// Four part version number in the form of Major.Minor[.Build[.Revision]].
        /// Revision is ONLY METADATA to MSI and will be ignored by Windows Installer.
        /// If a Revision is specified, it will be ignored by upgrade detection as well and the MSI will install as a separate app.
        /// If you need to upgrade revisions use allow_same_version_upgrades but note that this will also allow downgrades (e.g. 1.0.0.15 -> 1.0.0.3).
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        /// <summary>
        /// This implies <see cref="AllowSameVersionUpgrades"/>
        /// </summary>
        [JsonPropertyName("allow_downgrades")]
        public bool? AllowDowngrades { get; set; }

        [JsonPropertyName("allow_same_version_upgrades")]
        public bool? AllowSameVersionUpgrades { get; set; }

        [JsonPropertyName("schedule_reboot")]
        public bool? Reboot { get; set; }

        /// <summary>
        /// Sets admin or per-user installation. Possible values are "machine" (admin) and "user".
        /// </summary>
        [JsonPropertyName("install_scope")]
        public string? InstallScope { get; set; }

        /// <summary>
        /// Sets the UI mode of the installer. Possible values are "full", "basic" and "none".
        /// </summary>
        [JsonPropertyName("ui_mode")]
        public string? UiMode { get; set; }

        [JsonPropertyName("out_file")]
        public string? OutFileName { get; set; }
    }

    [JsonPropertyName("meta")]
    public MetadataConfig? Metadata { get; set; }
    public class MetadataConfig
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("author")]
        public string? Manufacturer { get; set; }

        /// <summary>
        /// License RTF file to be displayed/accepted during installation.
        /// </summary>
        [JsonPropertyName("license_file")]
        public string? LicenseFilePath { get; set; }

        /// <summary>
        /// Image at the top of the installer UI. Must be a 493x58 image file.
        /// </summary>
        [JsonPropertyName("banner_image")]
        public string? BannerImagePath { get; set; }

        /// <summary>
        /// Image at the side of the installer UI. Must be a 493x312 image file.
        /// </summary>
        [JsonPropertyName("dialog_image")]
        public string? DialogImagePath { get; set; }

        /// <summary>
        /// This is the image displayed in the Control Panel/Settings app. Has to be a windows icon file.
        /// </summary>
        [JsonPropertyName("product_icon")]
        public string? ProductIconPath { get; set; }

        [JsonPropertyName("help_url")]
        public string? HelpUrl { get; set; }

        [JsonPropertyName("about_url")]
        public string? AboutUrl { get; set; }

        [JsonPropertyName("update_url")]
        public string? UpdateUrl { get; set; }

        /// <summary>
        /// Removes the Change button from the Control Panel/Settings app. Can still be done using msiexec.
        /// </summary>
        [JsonPropertyName("forbid_modify")]
        public bool? ForbidModify { get; set; }

        /// <summary>
        /// Disables the Repair button from the Control Panel/Settings app. Can still be done using msiexec.
        /// </summary>
        [JsonPropertyName("forbid_repair")]
        public bool? ForbidRepair { get; set; }

        /// <summary>
        /// Removes the Uninstall button from the Control Panel/Settings app. Program can still be uninstalled using the change button or msiexec.
        /// </summary>
        [JsonPropertyName("forbid_uninstall")]
        public bool? ForbidUninstall { get; set; }

        /// <summary>
        /// Registers the software as a System Component, hiding it from the Control Panel/Settings app. Does not impact msiexec usage.
        /// </summary>
        [JsonPropertyName("hide_program_entry")]
        public bool? HideProgramEntry { get; set; }
    }

    [JsonPropertyName("install")]
    public InstallationConfig? Installation { get; set; }
    public class InstallationConfig
    {
        /// <summary>
        /// Target installation directory. May contain environment variables like %ProgramFiles% or %LocalAppData%.
        /// </summary>
        [JsonPropertyName("install_dir")]
        public string? Destination { get; set; }

        /// <summary>
        /// Single files to be included in the installation.
        /// </summary>
        [JsonPropertyName("source_files")]
        public List<string> Files { get; set; } = [];

        /// <summary>
        /// Directories to be included in the installation. Must contain a wildcard in the form of <c>*.*</c> as the filename/>
        /// </summary>
        [JsonPropertyName("source_dirs")]
        public List<string> Dirs { get; set;  } = [];

        [JsonPropertyName("source_dirs_are_recursive")]
        public bool? DirsRecursive { get; set; }

        /// <summary>
        /// Configuration for code signing the MSI and installed files.
        /// </summary>
        [JsonPropertyName("signing")]
        public SigningConfig? Signing { get; set; }
        public class SigningConfig
        {
            /// <summary>
            /// Name of the PFX File or the certificate in the certificate store to use for signing.
            /// </summary>
            [JsonPropertyName("cert_name")]
            [JsonRequired]
            public string CertificateName { get; set; } = "";

            /// <summary>
            /// PFX file password. Shouldn't be provided in the config for security reasons.
            /// </summary>
            [JsonIgnore] 
            public string? Password { get; set; } = null;

            /// <summary>
            /// Description to include in the signature. E.g. My Company MyApp.
            /// </summary>
            [JsonPropertyName("description")]
            public string? Description { get; set; }

            /// <summary>
            /// URL to the timestamping server.
            /// </summary>
            [JsonPropertyName("time_url")]
            public string? TimeUrl { get; set; }

            /// <summary>
            /// Type of the certificate store. May be "sha1", "name" or "pfx". Defaults to "pfx".
            /// </summary>
            [JsonPropertyName("store_type")]
            public string? StoreType { get; set; }

            /// <summary>
            /// Hash algorithm to use for signing. May be "sha1" or "sha256". Defaults to "sha256".
            /// </summary>
            [JsonPropertyName("algorithm")]
            public string? HashAlgorithm { get; set; }

            /// <summary>
            /// Additional arguments to pass to signtool
            /// </summary>
            [JsonPropertyName("extra_arguments")]
            public string? ExtraArguments { get; set; }

            /// <summary>
            /// Folder where signtool.exe is located. If not specified, it is assumed to be in the PATH. May include multiple ; separated paths.
            /// </summary>
            [JsonPropertyName("signtool_location")]
            public string? SignToolLocation { get; set; }

            /// <summary>
            /// Sign all files embedded in the MSI as well. This includes Program files and DLLs of the App you're shipping unless they are already signed.
            /// Note: The signing is performed in a subdirectory or in place.
            /// </summary>
            [JsonPropertyName("sign_embedded")]
            public bool? SignEmbeddedFiles { get; set; }
        }

        [JsonPropertyName("env_vars")]
        public List<EnvVarConfig> EnvironmentVariables { get; set;} = [];
        public class EnvVarConfig
        {
            [JsonPropertyName("name")]
            [JsonRequired]
            public string Name { get; set; } = "";

            /// <summary>
            /// Value to set the environment variable to. '@' gets replaced with the installation directory.
            /// </summary>
            [JsonPropertyName("value")]
            [JsonRequired]
            public string Value { get; set; } = "";

            /// <summary>
            /// May be "all", "prefix" or "suffix". Defaults to "all". "suffix" is commonly used for PATH modifications.
            /// </summary>
            [JsonPropertyName("part")]
            public string? Part { get; set; }
        }

        [JsonPropertyName("shortcuts")]
        public List<ShortcutConfig> Shortcuts { get; set;} = [];
        public class ShortcutConfig
        {

            /// <summary>
            /// Files are searched by the end of their filename. So "app.exe" would match "myapp.exe".
            /// </summary>
            [JsonPropertyName("target")]
            [JsonRequired]
            public string TargetFile { get; set; } = "";
            /// <summary>
            /// May include values like %Desktop%, defaults to the Program Menu.
            /// </summary>
            [JsonPropertyName("location")]
            public string? Location { get; set; }

            /// <summary>
            /// Name of the shortcut. Defaults to the App's name.
            /// </summary>
            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }
    }

    public static Config? FromToml(string toml)
    {
        TomlMetadataStore store = new();
        TomlSerializerOptions options = new()
        {
            MetadataStore = store
        };

        if (TomlSerializer.TryDeserialize(toml, out Config? config, options))
        {
            config.TomlMetadataStore = store;
        }

        return config;
    }

    public string ToToml()
    {
        TomlSerializerOptions options = new()
        {
            MetadataStore = TomlMetadataStore
        };
        return TomlSerializer.Serialize(this, options);
    }
}

