// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Exception that gets thrown when not all stream processor states to be persisted are not in the same scope.
/// </summary>
public class NotAllStreamProcessorStatesInSameScope : Exception
{
    public NotAllStreamProcessorStatesInSameScope(ScopeId expectedScope)
        : base($"Expected all stream processor states to be persisted to be in the same scope {expectedScope}")
    {
    }
}
