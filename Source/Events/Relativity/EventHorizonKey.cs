/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Concepts;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Relativity
{

    /// <summary>
    /// A key to identify a particular <see cref="Application" /> and <see cref="BoundedContext" /> combination
    /// </summary>
    public class EventHorizonKey : Value<EventHorizonKey>
    {
        /// <summary>
        /// Returns the Application
        /// </summary>
        /// <value></value>
        public Dolittle.Applications.Application Application { get; }
        /// <summary>
        /// Returns the BoundedContext
        /// </summary>
        /// <value></value>
        public BoundedContext BoundedContext { get; }

        /// <summary>
        /// Instantiates an instance of <see cref="EventHorizonKey" />
        /// </summary>
        /// <param name="application"></param>
        /// <param name="boundedContext"></param>
        public EventHorizonKey(Dolittle.Applications.Application application, BoundedContext boundedContext)
        {
            Application = application;
            BoundedContext = boundedContext;
        }

        /// <summary>
        /// Returns a string representation of the key
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Application} {BoundedContext}";
        }
    }
}