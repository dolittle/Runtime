// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

public class DolittleConfigurations : ReadOnlyDictionary<string, string>
{
    public TConfig Get<TConfig>(string configName, JsonSerializerOptions options = default)
        => JsonSerializer.Deserialize<TConfig>(this[configName], options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    public TConfig GetFor<TConfig>(JsonSerializerOptions options = default)
        where TConfig : class
        => Get<TConfig>(typeof(TConfig).GetTypeInfo().GetCustomAttribute<NameAttribute>()!.Name, options);

    public DolittleConfigurations(IDictionary<string, string> dictionary)
        : base(dictionary)
    {
    }
}
