// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration.Parsing;

public interface IParseConfigurationObjects
{
    bool TryParseFrom<TOptions>(IConfigurationSection configuration, out TOptions parsed)
        where TOptions : class;
}
