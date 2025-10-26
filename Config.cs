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

        [DataMember(Name = "platform")]
        public string? Platform { get; set; }

        [DataMember(Name = "version")]
        public string? Version { get; set; }

        [DataMember(Name = "schedule_reboot")]
        public bool? Reboot { get; set; }

        [DataMember(Name = "install_scope")]
        public string? InstallScope { get; set; }

        [DataMember(Name = "ui_mode")]
        public string? UiMode { get; set; }

        [DataMember(Name = "out_file")]
        public string? OutFileName { get; set; }
    }

    [DataMember(Name = "metadata")]
    public MetadataConfig? Metadata { get; set; }
    public class MetadataConfig : ITomlMetadataProvider
    {
        public TomlPropertiesMetadata? PropertiesMetadata { get; set; }

        [DataMember(Name = "display_name")]
        public string? DisplayName { get; set; }

        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "license_file")]
        public string? LicenseFilePath { get; set; }
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

