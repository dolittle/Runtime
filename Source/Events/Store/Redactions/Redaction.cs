﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Dolittle.Runtime.Events.Store.Redactions;

/// <summary>
/// Event that triggers redaction of the given personal data
/// It will target the given event type and redact the properties specified within the EventSourceId of the event
/// </summary>
public class Redaction
{
    public const string RedactedPrefix = "de1e7e17-bad5-da7a";

    public EventSourceId EventSourceId { get; }
    public Event Details { get; }
    public EventLogSequenceNumber EventLogSequenceNumber { get; }
    public Guid TypeId { get; }

    public Redaction(EventSourceId eventSourceId, Event @event, EventLogSequenceNumber eventLogSequenceNumber,
        Guid typeId)
    {
        EventSourceId = eventSourceId;
        Details = @event;
        EventLogSequenceNumber = eventLogSequenceNumber;
        TypeId = typeId;
    }

    public class Event
    {
        public required string EventId { get; init; }
        public required string EventAlias { get; init; }

        /// <summary>
        /// The properties that will be redacted, and the replacement values.
        /// Can be null, in which case the properties will be redacted with a default value
        /// </summary>
        public required Dictionary<string, object?> RedactedProperties { get; init; }

        public required string RedactedBy { get; init; }
        public required string Reason { get; init; }

        public bool IsValid => !string.IsNullOrWhiteSpace(EventId)
                               && !string.IsNullOrWhiteSpace(EventAlias)
                               && RedactedProperties.Count > 0
                               && !string.IsNullOrWhiteSpace(RedactedBy)
                               && !string.IsNullOrWhiteSpace(Reason);
    }

    public static bool TryGet(CommittedEvent evt, [NotNullWhen(true)] out Redaction? redaction)
    {
        redaction = default;
        if (!IsRedactionId(evt.Type.Id))
        {
            return false;
        }

        try
        {
            var payload = JsonSerializer.Deserialize<Event>(evt.Content);
            if (payload is not { IsValid: true })
            {
                return false;
            }

            if (!Guid.TryParse(payload.EventId, out var redactedTypeId))
            {
                return false;
            }

            if (IsRedactionId(redactedTypeId))
            {
                // Cannot redact a redaction. This is to prevent removing logs of what has been redacted
                // As redactions themselves should not contain PII, this should not be a problem
                return false;
            }


            redaction = new Redaction(evt.EventSource, payload, evt.EventLogSequenceNumber, redactedTypeId);
            return true;
        }
        catch // Bad payload, ignore
        {
            return false;
        }
    }

    static bool IsRedactionId(Guid id) =>
        id.ToString().StartsWith(RedactedPrefix, StringComparison.InvariantCultureIgnoreCase);
}
