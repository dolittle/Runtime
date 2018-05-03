/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents the context when storing events
    /// </summary>
    public class EventStorageContext
    {
        /// <summary>
        /// Gets or sets the <see cref="DateTimeOffset">date time</see> for the context
        /// </summary>
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId"/> for the context
        /// </summary>
        public TenantId Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ApplicationName"/>
        /// </summary>
        public ApplicationName Application { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BoundedContext"/>
        /// </summary>
        public BoundedContext BoundedContext { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventSourceId"/>
        /// </summary>
        public EventSourceId EventSourceId { get; set; }
    }
}