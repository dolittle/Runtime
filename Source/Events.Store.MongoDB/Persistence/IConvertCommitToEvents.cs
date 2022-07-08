// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Persistence;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Defines a converter that converts <see cref="Commit"/>.
/// </summary>
public interface IConvertCommitToEvents
{
    /// <summary>
    /// Converts the <see cref="Commit"/> to an <see cref="IEnumerable{T}"/> of <see cref="MongoDB.Events.Event"/>
    /// </summary>
    /// <param name="commit"></param>
    /// <returns></returns>
    IEnumerable<MongoDB.Events.Event> ToEvents(Commit commit);
}