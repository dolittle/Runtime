// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients.given;

public class no_known_clients
{
    protected static KnownClients known_clients;

    Establish context = () => known_clients = new KnownClients(new StaticInstancesOf<IKnowAboutClients>());
}