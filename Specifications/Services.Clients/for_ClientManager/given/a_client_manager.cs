// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.given;

public class a_client_manager : all_dependencies
{
    protected static ClientManager client_manager;

    Establish context = () => client_manager = new ClientManager(call_invoker_manager.Object);
}