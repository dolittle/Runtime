// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

import { autoinject } from 'aurelia-dependency-injection';
import { HttpClient } from 'aurelia-fetch-client';
import { Router } from 'aurelia-router';

import { Globals } from './globals';

import { IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';

import { INavLink } from 'office-ui-fabric-react/lib/Nav';

// http://www.jeremyg.net/entry/create-a-menu-with-child-routes-using-aurelia

@autoinject
export class Navigation {
    tenants: any[] = [];

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

    constructor(
        private _router: Router,
        private _httpClient: HttpClient,
        private _globals: Globals) {
    }

    tenantSelected(event: any, option?: IDropdownOption, index?: number): void {
        const tenant = Guid.parse(option?.key as string);
        this._globals.currentTenant = tenant;
    }

    async refreshTenants() {
        await this.loadTenants();
    }

    async attached() {
        await this.loadTenants();
    }

    itemClicked(element: any, link: INavLink) {
    }

    async loadTenants() {
        const result = await this._httpClient.get('/api/Tenants');
        const tenants = await result.json() as any as any[];

        tenants.forEach((tenant: any) => {
            this.tenants.push({
                key: tenant.value,
                text: tenant.value,
            });
        });
    }
}
