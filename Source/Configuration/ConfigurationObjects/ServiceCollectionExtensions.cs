// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects;

public class ConfigurationConverter : JsonConverter<IConfiguration>
{
    public override IConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IConfiguration value, JsonSerializerOptions options)
    {
        if (value is IConfigurationSection section)
        {
            if (section.Value is null)
            {
                writer.WriteStartObject(section.Key);
            }
            else
            {
                writer.WriteString(section.Key, section.Value);
                return;
            }
        }
        else
        {
            writer.WriteStartObject();
        }

        foreach (var child in value.GetChildren())
        {
            Write(writer, child, options);
        }

        writer.WriteEndObject();
    }
}
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDolittleConfigurations(this IServiceCollection services, IConfiguration dolittleConfiguration)
    {
        var json = new ConvertDolittleConfigurationToJson().Convert(dolittleConfiguration);
        var x2 = JsonSerializer.Deserialize<EndpointsConfiguration>(json["endpoints"], new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        var x3 = JsonSerializer.Deserialize<TenantsConfiguration>(json["tenants"], new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        return services;
    }
}
