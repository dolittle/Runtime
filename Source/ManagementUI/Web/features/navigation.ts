// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { autoinject } from 'aurelia-dependency-injection';

// http://www.jeremyg.net/entry/create-a-menu-with-child-routes-using-aurelia

@autoinject
export class Navigation {

    groups = [
        {
            links: [
                {
                    name: 'General',
                    links: [
                        {
                            name: 'Overview',
                            key: 'overview',
                            url: 'general/overview'
                        },
                        {
                            name: 'Metrics',
                            key: 'metrics',
                            url: 'general/metrics'
                        }
                    ]
                },
                {
                    name: 'Connections',
                    links: [
                        {
                            name: 'Runtimes',
                            key: 'runtimes',
                            url: 'connections/runtimes'
                        },
                        {
                            name: 'Heads',
                            key: 'heads',
                            url: 'connections/heads'
                        }
                    ],
                },
                {
                    name: 'Event Store',
                    links: [
                        {
                            name: 'Failing Partitions',
                            key: 'failing-partitions',
                            url: 'event-store/failing-partitions'
                        },
                        {
                            name: 'Aggregates',
                            key: 'aggregates',
                            url: 'event-store/aggregates'
                        },
                        {
                            name: 'Event Log',
                            key: 'log',
                            url: 'event-store/log'
                        },
                        {
                            name: 'Streams',
                            key: 'streams',
                            url: 'event-store/streams'
                        },
                        {
                            name: 'Schemas',
                            key: 'schemas',
                            url: 'event-store/schemas'
                        }
                    ]
                },
                {
                    name: 'TimeSeries',
                    links: [
                        {
                            name: 'Connectors',
                            key: 'connectors',
                            url: 'time-series/connectors'
                        },
                        {
                            name: 'Observers',
                            key: 'observers',
                            url: 'time-series/observers'
                        },
                        {
                            name: 'Identities',
                            key: 'identities',
                            url: 'time-series/identities'
                        }
                    ]
                }
            ],

        },
    ];
}
