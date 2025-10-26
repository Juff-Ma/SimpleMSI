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
    public class GeneralConfig
    {
        [DataMember(IsRequired = true, Name = "guid")]
        public string Guid { get; set; } = System.Guid.Empty.ToString();

        [DataMember(IsRequired = true, Name = "name")]
        public string Name { get; set; } = "";

        [DataMember(Name = "platform")]
        public string Platform { get; set; } = "x64";
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

