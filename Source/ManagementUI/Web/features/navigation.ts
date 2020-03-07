// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

import { autoinject } from 'aurelia-dependency-injection';
import { HttpClient } from 'aurelia-fetch-client';

import { Globals } from './globals';

import { IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';

@autoinject
export class Navigation {
    tenants: any[] = [];

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
                    key: 'aggregates',
                    name: 'Aggregates',
                    url: '#'
                },
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
                },
                {
                    key: 'identities',
                    name: 'Identities',
                    url: '#'
                }
            ],
            name: 'TimeSeries',
            url: '#',
        }
    ];

    constructor(
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
