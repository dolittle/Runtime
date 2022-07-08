// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Configuration;

namespace Microservices;

[Configuration("microservices")]
public class MicroservicesConfiguration : Dictionary<Guid, MicroserviceConfiguration>
{
}
