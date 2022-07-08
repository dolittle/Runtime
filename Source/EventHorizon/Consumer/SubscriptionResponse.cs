// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents the response of subscription request.
/// </summary>
public class SubscriptionResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class.
    /// </summary>
    /// <param name="consentId">The <see cref="ConsentId" />.</param>
    SubscriptionResponse(ConsentId consentId)
    {
        Success = true;
        ConsentId = consentId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionResponse"/> class.
    /// </summary>
    /// <param name="failure">The <see cref="Failure"/>.</param>
    SubscriptionResponse(Failure failure) => Failure = failure;


    /// <summary>
    /// Creates a succeeded <see cref="SubscriptionResponse" />.
    /// </summary>
    /// <param name="consent">The <see cref="ConsentId" />.</param>
    /// <returns>The <see cref="SubscriptionResponse" />.</returns>
    public static SubscriptionResponse Succeeded(ConsentId consent) => new(consent);

    /// <summary>
    /// Creates a failed <see cref="SubscriptionResponse" />.
    /// </summary>
    /// <param name="failure">The <see cref="Failure" />.</param>
    /// <returns>The <see cref="SubscriptionResponse" />.</returns>
    public static SubscriptionResponse Failed(Failure failure) => new(failure);

    /// <summary>
    /// Gets a value indicating whether the subscription is fine.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the <see cref="ConsentId" />.
    /// </summary>
    public ConsentId ConsentId { get; }

    /// <summary>
    /// Gets the reason for why the subscription failed.
    /// </summary>
    public Failure Failure { get; }
}