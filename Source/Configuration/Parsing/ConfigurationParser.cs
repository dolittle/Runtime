// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
            if (section.Value == "null" ||
                double.TryParse(section.Value, NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var _) ||
                bool.TryParse(section.Value, out var _))
            {
                stream.Write(section.Value.ToLower(CultureInfo.InvariantCulture));
                return;
            }
            
            stream.Write('"');
            stream.Write(section.Value);
            stream.Write('"');
            return;
        }

        var children = section.GetChildren().ToList();

        var isArray = children.Select(_ => _.Key).All(_ => int.TryParse(_, NumberStyles.None, CultureInfo.InvariantCulture, out var _));

        if (isArray)
        {
            children = children.OrderBy(_ => int.Parse(_.Key, CultureInfo.InvariantCulture)).ToList();
            stream.Write('[');
        }
        else
        {
            stream.Write('{');
        }

        for (var i = 0; i < children.Count; i++)
        {
            if (i > 0)
            {
                stream.Write(',');
            }

            var child = children[i];

            if (!isArray)
            {
                stream.Write('"');
                stream.Write(child.Key);
                stream.Write('"');
                stream.Write(':');
            }
            
            WriteSectionToStream(child, stream);
        }

        if (isArray)
        {
            stream.Write(']');
        }
        else
        {
            stream.Write('}');
        }
    }
}
