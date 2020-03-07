// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export class Navigation {
    groups = [
        {
            isExpanded: true,
            links: [
                {
                    key: 'runtimes',
                    name: 'Runtimes',
                    url: '#'
                },
                {
                    key: 'heads',
                    name: 'Heads',
                    url: '#'
                }
            ],
            name: 'Connections',
            url: '#',
        },
        {
            isExpanded: true,
            links: [
                {
                    key: 'log',
                    name: 'Event Log',
                    url: '#'
                },
                {
                    key: 'streams',
                    name: 'Streams',
                    url: '#'
                },
                {
                    key: 'schemas',
                    name: 'Schemas',
                    url: '#'
                }
            ],
            name: 'Event Store',
            url: '#',
        },
        {
            isExpanded: true,
            links: [
                {
                    key: 'connectors',
                    name: 'Connectors',
                    url: '#'
                },
                {
                    key: 'observers',
                    name: 'Observers',
                    url: '#'
                }
            ],
            name: 'TimeSeries',
            url: '#',
        }
    ];
}
