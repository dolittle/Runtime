// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

public class ConvertDolittleConfigurationToJson : IConvertDolittleConfigurationToJson
{
    public IDictionary<string, string> Convert(IConfiguration configuration)
        => configuration.GetChildren().ToDictionary(_ => _.Key, _ => Serialize(_).ToString());
    
    static JToken Serialize(IConfiguration config)
    {
        var obj = new JObject();

        foreach (var child in config.GetChildren())
        {
            if (child.Path.EndsWith(":0", StringComparison.InvariantCulture))
            {
                var arr = new JArray();

                foreach (var arrayChild in config.GetChildren())
                {
                    arr.Add(Serialize(arrayChild));
                }

                return arr;
            }
            obj.Add(child.Key, Serialize(child));
        }

        if (obj.HasValues || config is not IConfigurationSection section || section.Value == null)
        {
            return obj;
        }
        if (bool.TryParse(section.Value, out var boolean))
        {
            return new JValue(boolean);
        }
        if (long.TryParse(section.Value, out var integer))
        {
            return new JValue(integer);
        }
        if (decimal.TryParse(section.Value, out var real))
        {
            return new JValue(real);
        }
        return new JValue(section.Value);

    }
}
