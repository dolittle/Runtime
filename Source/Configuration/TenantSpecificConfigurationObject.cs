// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Configuration;

public abstract class TenantSpecificConfigurationObject<TConfiguration> : Dictionary<Guid, TConfiguration>
    where TConfiguration : class
{
}
