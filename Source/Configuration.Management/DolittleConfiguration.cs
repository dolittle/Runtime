// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Configuration.Management;

[Singleton]
public class DolittleConfiguration : IDolittleConfiguration
{
    readonly JsonSerializerSettings _jsonSerializerSettings = new (){NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore,Culture = CultureInfo.InvariantCulture};
    readonly JsonSerializer _jsonSerializer;
    readonly IAllConfigurations _allConfigurations;
    readonly JObject _fullyParsedConfig;
    
    public DolittleConfiguration(IAllConfigurations allConfigurations)
    {
        _jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        _jsonSerializerSettings.Converters.Add(new ExpandoObjectConverter());
        _jsonSerializer = JsonSerializer.CreateDefault(_jsonSerializerSettings);
        _allConfigurations = allConfigurations;
        _fullyParsedConfig = GenerateDolittleConfig();
        Config = AsExpandoObject(_fullyParsedConfig);
    }
    
    /// <inheritdoc />
    public ExpandoObject Config { get; }

    /// <inheritdoc />
    public ExpandoObject Include(string include)
    {
        var config = _fullyParsedConfig.DeepClone();
        var token = config.SelectToken(include, errorWhenNoMatch: false);
        return AsExpandoObject(token is not null ? RemoveEmptyChildren(token) : new JObject());
    }

    /// <inheritdoc />
    public ExpandoObject Ignoring(IEnumerable<string> ignores)
    {
        var config = (JObject)_fullyParsedConfig.DeepClone();
        foreach (var ignore in ignores)
        {
            config.Remove(ignore);
        }
        return AsExpandoObject(RemoveEmptyChildren(config));
    }

    ExpandoObject AsExpandoObject(JToken token) => JsonConvert.DeserializeObject<ExpandoObject>(token.ToString(Formatting.Indented), _jsonSerializerSettings)!;
    
    static T1 Merge<T1, T2>(T1 first, T2 second)
        where T1 : JContainer
        where T2 : JContainer
    {
        first.Merge(second, new JsonMergeSettings{PropertyNameComparison = StringComparison.InvariantCultureIgnoreCase, MergeArrayHandling = MergeArrayHandling.Union, MergeNullValueHandling = MergeNullValueHandling.Ignore});
        return first;
    }

    JObject MergedObject(JObject root, string propertyName, string[] childPropertyNames, object o)
    {
        if (childPropertyNames.Length == 0)
        {
            if (root.TryGetValue(propertyName, out var token))
            {
                var tokenAsObject = (JContainer) token;
                token = Merge(tokenAsObject, JObject.FromObject(o, _jsonSerializer));
            }
            else
            {
                token = JObject.FromObject(o, _jsonSerializer);
            }
            root[propertyName] = token;
            return root;
        }

        root[propertyName] = MergedObject(root.GetValue(propertyName) as JObject ?? new JObject(), childPropertyNames[0], childPropertyNames.Skip(1).ToArray(), o);
        return root;
    }
    
    JObject GenerateDolittleConfig()
    {
        var result = new JObject();
        foreach (var (path, config) in _allConfigurations.Configurations)
        {
            var (propertyName, childPropertyNames) = GetPropertyNameAndChildren(path);
            result = MergedObject(result, propertyName, childPropertyNames, config);
        }

        foreach (var (path, configsPerTenant) in _allConfigurations.TenantConfigurations)
        {
            foreach (var (tenant, config) in configsPerTenant)
            {
                var (propertyName, childPropertyNames) = GetPropertyNameAndChildren(ConfigurationPath.Combine("tenants", tenant.Value.ToString(), path));
                result = MergedObject(result, propertyName, childPropertyNames, config);
            }
        }

        return (JObject)RemoveEmptyChildren(result);
    }

    static JToken RemoveEmptyChildren(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
            {
                var copy = new JObject();
                foreach (var prop in token.Children<JProperty>())
                {
                    var child = prop.Value;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (child.Type != JTokenType.Object || child.HasValues)
                    {
                        copy.Add(prop.Name, child);
                    }
                }
                return copy;
            }

            case JTokenType.Array:
            {
                var copy = new JArray();
                foreach (var item in token.Children())
                {
                    var child = item;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (child.Type != JTokenType.Object || child.HasValues)
                    {
                        copy.Add(child);
                    }
                }
                return copy;
            }

            default:
                return token;
        }
    }

    static (string propertyName, string[] childPropertyNames) GetPropertyNameAndChildren(string path)
    {
        var allProps = path.Split(ConfigurationPath.KeyDelimiter);
        return (allProps[0], allProps.Skip(1).ToArray());
    }
}
