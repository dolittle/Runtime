// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.Configuration.Legacy;

public class LegacyConfigurationProvider : ConfigurationProvider
{
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
            var fileConfiguration = fileBuilder.Build();
            MapConfiguration(file.Name, fileConfiguration);
        }
    }

    void MapConfiguration(string file, IConfiguration configuration)
    {
        switch (file)
        {
            case "endpoints.json":
                MapEndpoints(configuration);
                break;
        }
    }

    void MapEndpoints(IConfiguration endpoints)
    {
        Data["dolittle:runtime:endpoints:public:port"] = endpoints["public:port"];
        Data["dolittle:runtime:endpoints:public:enabled"] = endpoints["public:enabled"];
        Data["dolittle:runtime:endpoints:private:port"] = endpoints["private:port"];
        Data["dolittle:runtime:endpoints:private:enabled"] = endpoints["private:enabled"];
        Data["dolittle:runtime:endpoints:management:port"] = endpoints["management:port"];
        Data["dolittle:runtime:endpoints:management:enabled"] = endpoints["management:enabled"];
    }
}
