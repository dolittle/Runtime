// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

public interface IEvent<T> where T : IEvent<T>
{
    public ulong EventLogSequenceNumber { get; }
    public ulong StreamPosition { get; }
    
    public static abstract Expression<Func<T, object>> StreamPositionExpression { get; }
}

