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
                            url: 'general/overview',
                            icon: 'AutoRacing'
                        },
                        {
                            name: 'Metrics',
                            key: 'metrics',
                            url: 'general/metrics',
                            icon: 'Chart'
                        }
                    ]
                },
                {
                    name: 'Connections',
                    links: [
                        {
                            name: 'Runtimes',
                            key: 'runtimes',
                            url: 'connections/runtimes',
                            icon: 'Communications'
                        },
                        {
                            name: 'Heads',
                            key: 'heads',
                            url: 'connections/heads',
                            icon: 'Communications'
                        }
                    ],
                },
                {
                    name: 'Event Store',
                    links: [
                        {
                            name: 'Failing Partitions',
                            key: 'failing-partitions',
                            url: 'event-store/failing-partitions',
                            icon: 'ErrorBadge'
                        },
                        {
                            name: 'Aggregates',
                            key: 'aggregates',
                            url: 'event-store/aggregates',
                            icon: 'AssessmentGroup'
                        },
                        {
                            name: 'Event Log',
                            key: 'log',
                            url: 'event-store/log',
                            icon: 'CustomList'
                        },
                        {
                            name: 'Streams',
                            key: 'streams',
                            url: 'event-store/streams',
                            icon: 'DrillExpand'
                        },
                        {
                            name: 'Schemas',
                            key: 'schemas',
                            url: 'event-store/schemas',
                            icon: 'EntryView'
                        }
                    ]
                },
                {
                    name: 'TimeSeries',
                    links: [
                        {
                            name: 'Connectors',
                            key: 'connectors',
                            url: 'time-series/connectors',
                            icon: 'Plug'
                        },
                        {
                            name: 'Observers',
                            key: 'observers',
                            url: 'time-series/observers',
                            icon: 'RedEye'
                        },
                        {
                            name: 'Identities',
                            key: 'identities',
                            url: 'time-series/identities',
                            icon: 'IDBadge'
                        }
                    ]
                }
            ],

        },
    ];
}
