// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Dolittle.Runtime.Configuration;

public static class Constants
{
    public static readonly string DolittleConfigSectionRoot = ConfigurationPath.Combine("dolittle", "runtime");

    public static string CombineWithDolittleConfigRoot(params string[] sections)
        => ConfigurationPath.Combine(sections.Prepend(DolittleConfigSectionRoot));
}
