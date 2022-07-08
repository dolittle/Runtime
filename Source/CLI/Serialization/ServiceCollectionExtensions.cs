// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Dolittle.Runtime.CLI.Serialization;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services related to serialization to the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to add serialization services to.</param>
    public static void AddSerializers(this ServiceCollection services)
    {
        var conceptConverter = new ConceptConverter();
        var settings = new JsonSerializerSettings
        {
            Converters = new[]
            {
                conceptConverter
            }
        };
        services.AddSingleton<ISerializer>(new Serializer(settings));
        services.AddSingleton(settings);
        services.AddSingleton(conceptConverter);
    }
}

public class Serializer : ISerializer
{
    readonly JsonSerializerSettings _settings;

    public Serializer(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    public T FromJson<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, _settings);

    public object FromJson(Type type, string json)
        => JsonConvert.DeserializeObject(json, type, _settings);

    public string ToJson(object obj, Formatting formatting = Formatting.None)
        => JsonConvert.SerializeObject(obj, formatting, _settings);
}
public interface ISerializer
{
    T FromJson<T>(string json);
    object FromJson(Type type, string json);

    string ToJson(object obj, Formatting formatting = Formatting.None);
}
