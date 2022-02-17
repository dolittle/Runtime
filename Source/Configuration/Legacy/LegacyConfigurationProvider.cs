// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration.Legacy;

public class LegacyConfigurationProvider : ConfigurationProvider
{
    static string _delimiter = ConfigurationPath.KeyDelimiter;
    readonly IFileProvider _fileProvider;

    public LegacyConfigurationProvider(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public override void Load()
    {
        foreach (var file in _fileProvider.GetDirectoryContents("/"))
        {
            var fileBuilder = new ConfigurationBuilder();
            fileBuilder.AddJsonFile(_fileProvider, file.Name, false, false);
            var configuration = fileBuilder.Build();
            MapData(file.Name, configuration); 
        }
    }

    void MapData(string file, IConfiguration config)
    {
        foreach (var kvp in GetData($"dolittle{_delimiter}runtime{_delimiter}{Path.GetFileNameWithoutExtension(file)}", config))
        {
            Data.Add(kvp);
        }
    }

    static IEnumerable<KeyValuePair<string, string>> GetData(string rootPath, IConfiguration config)
        => config.AsEnumerable()
            .Where(keyAndValue => keyAndValue.Value != null || !config.GetSection(keyAndValue.Key).GetChildren().Any())
            .Select(keyAndValue => new KeyValuePair<string,string>($"{rootPath}{_delimiter}{keyAndValue.Key}", keyAndValue.Value));
}
