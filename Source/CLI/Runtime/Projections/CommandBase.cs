// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.CLI.Serialization;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

public abstract class CommandBase : Runtime.CommandBase
{
    protected CommandBase(ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer) 
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
    }
}
