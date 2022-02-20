// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Configuration;

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
        parsed = default;
        return true;
    }
}
