// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.given;

public class all_dependencies
{
    protected static Mock<IChannels> call_invoker_manager;

    Establish context = () => call_invoker_manager = new Mock<IChannels>();
}