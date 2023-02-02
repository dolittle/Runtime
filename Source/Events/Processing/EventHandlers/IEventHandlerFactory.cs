// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using ReverseCallDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines a factory for <see cref="IEventHandler"/>.
/// </summary>
public interface IEventHandlerFactory
{
    /// <summary>
    /// Creates an <see cref="IEventHandler"/>.
    /// </summary>
    /// <param name="arguments">The <see cref="EventHandlerRegistrationArguments"/>.</param>
    /// <param name="dispatcher">The <see cref="ReverseCallDispatcher"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IEventHandler"/>.</returns>
    IEventHandler Create(EventHandlerRegistrationArguments arguments, ReverseCallDispatcher dispatcher, CancellationToken cancellationToken);

    /// <summary>
    /// Creates an <see cref="FastEventHandler"/>.
    /// </summary>
    /// <param name="arguments">The <see cref="EventHandlerRegistrationArguments"/>.</param>
    /// <param name="implicitFilter">Whether filtering is implicit.</param>
    /// <param name="dispatcher">The <see cref="ReverseCallDispatcher"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IEventHandler"/>.</returns>
    // FastEventHandler CreateFast(EventHandlerRegistrationArguments arguments, bool implicitFilter, ReverseCallDispatcher dispatcher, CancellationToken cancellationToken);
}
