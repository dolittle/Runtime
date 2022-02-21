// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Configuration.Parsing;

/// <summary>
/// Represents an implementation of <see cref="IParseConfigurationObjects"/>.
/// </summary>
public class ConfigurationParser : IParseConfigurationObjects
{
    public bool TryParseFrom<TOptions>(IConfigurationSection configuration, out TOptions parsed)
        where TOptions : class
    {
        Console.WriteLine($"Parsing configuration from {configuration} to {typeof(TOptions)}");
        
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        WriteSectionToStream(configuration, writer);
        writer.Flush();

        stream.Seek(0, SeekOrigin.Begin);
        var allStuff = new StreamReader(stream).ReadToEnd();
        Console.WriteLine(allStuff);

        stream.Seek(0, SeekOrigin.Begin);
        var reader = new JsonTextReader(new StreamReader(stream));
        var serializer = JsonSerializer.Create();
        parsed = serializer.Deserialize<TOptions>(reader);
        return true;
    }

    static void WriteSectionToStream(IConfigurationSection section, TextWriter stream)
    {
        if (section.Value != default)
        {
            stream.Write(section.Value);
            return;
        }
        
        stream.Write("{");
        foreach (var child in section.GetChildren())
        {
            stream.Write('"');
            stream.Write(child.Key);
            stream.Write('"');
            stream.Write(':');
            
            WriteSectionToStream(child, stream);
            stream.Write(',');
        }
        stream.Write("}");
    }
}
