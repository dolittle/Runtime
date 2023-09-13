// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Actors;

public class SubscriptionError: Exception
{
    public SubscriptionError(string message) : base(message)
    {
    }
}
