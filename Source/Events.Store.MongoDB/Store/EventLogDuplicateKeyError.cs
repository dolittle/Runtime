// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Exception that gets thrown when a duplicate key error occures while writing an event to the event log.
    /// </summary>
    public class EventLogDuplicateKeyError : MongoException
    {
        const string TransientTransactionErrorLabel = "TransientTransactionError";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogDuplicateKeyError"/> class.
        /// </summary>
        /// <param name="eventLogVersion">The event log version of the event that failed during write.</param>
        public EventLogDuplicateKeyError(uint eventLogVersion)
            : base($"Duplicate key error inserting event with event log version '{eventLogVersion}'.")
        {
            AddErrorLabel(TransientTransactionErrorLabel);
        }
    }
}