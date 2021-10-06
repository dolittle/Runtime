// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.V6.Models.Events
{
    /// <summary>
    /// Represents the platform generated information about an event that is stored alongside the domain specific data in the event store.
    /// </summary>
    public class EventHorizonMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonMetadata"/> class.
        /// </summary>
        public EventHorizonMetadata()
        {
            FromEventHorizon = false;
            Received = DateTime.MinValue;
            Consent = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonMetadata"/> class.
        /// </summary>
        /// <param name="externalEventLogSequenceNumber">The Event's original event log sequence number if it came from EventHorizon.</param>
        /// <param name="received">The <see cref="DateTime" /> ƒor when this Event was received.</param>
        /// <param name="consent">The consent id of the subscription this Event was received from.</param>
        public EventHorizonMetadata(
            ulong externalEventLogSequenceNumber,
            DateTime received,
            Guid consent)
        {
            FromEventHorizon = true;
            ExternalEventLogSequenceNumber = externalEventLogSequenceNumber;
            Received = received;
            Consent = consent;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this event came from EventHorizon.
        /// </summary>
        public bool FromEventHorizon { get; set; }

        /// <summary>
        /// Gets or sets the origin event log sequence number of the event if it came from EventHorizon.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong ExternalEventLogSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> of when the Event was received.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Received { get; set; }

        /// <summary>
        /// Gets or sets the consent id.
        /// </summary>
        public Guid Consent { get; set; }
    }
}
