// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

import { autoinject } from 'aurelia-dependency-injection';
import { HttpClient } from 'aurelia-fetch-client';
import { PLATFORM } from 'aurelia-pal';
import { Router, RouterConfiguration } from 'aurelia-router';

import { IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';
import { Globals } from './globals';

@autoinject
export class App {
    tenants: any[] = [];
    router: any;

    constructor(private _globals: Globals, private _httpClient: HttpClient) {
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.options.pushState = true;
        config.map([
            { route: ['', '/'], name: 'Index', moduleId: PLATFORM.moduleName('index'), nav: true },
            { route: 'general/overview', name: 'General Overview', moduleId: PLATFORM.moduleName('general/overview'), nav: true },
            { route: 'connections/runtimes', name: 'Runtime Connections', moduleId: PLATFORM.moduleName('connections/runtimes'), nav: true }
        ]);

        this.router = router;
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
