// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

import { autoinject } from 'aurelia-dependency-injection';
import { PLATFORM } from 'aurelia-pal';
import { Router, RouterConfiguration } from 'aurelia-router';

import { IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';
import { Globals } from './globals';

import { TenantsClient } from '@dolittle/runtime.contracts.web/Tenancy.Management/Tenants_grpc_web_pb';
import { Tenant, TenantsRequest, TenantsResponse } from '@dolittle/runtime.contracts.web/Tenancy.Management/Tenants_pb';
import { ClientFactory } from './ClientFactory';

import * as grpc from "grpc";

@autoinject
export class App {
    tenants: any[] = [];
    router: any;

    private _client: TenantsClient;
    private _getTenantsCall: grpc.ClientReadableStream<TenantsResponse> = null as any;

    constructor(private _globals: Globals, private _clientFactory: ClientFactory) {
        this._client = _clientFactory.create(TenantsClient);
    }

    configureRouter(config: RouterConfiguration, router: Router) {
        config.options.pushState = true;
        config.map([
            { route: ['', '/'], name: 'Index', moduleId: PLATFORM.moduleName('index'), nav: true },
            { route: 'general/overview', name: 'General Overview', moduleId: PLATFORM.moduleName('general/overview'), nav: true },
            { route: 'connections/runtimes', name: 'Runtime Connections', moduleId: PLATFORM.moduleName('connections/runtimes'), nav: true },
            { route: 'connections/heads', name: 'Connected Heads', moduleId: PLATFORM.moduleName('connections/heads'), nav: true }
        ]);

        this.router = router;
    }

    tenantSelected(event: any, option?: IDropdownOption, index?: number): void {
        const tenant = Guid.parse(option?.key as string);
        this._globals.currentTenant = tenant;
    }

    attached() {
        const request = new TenantsRequest();

        this._getTenantsCall = this._client.getTenants(request);
        this._getTenantsCall.on('data', (response: TenantsResponse) => {
            const tenants = response.getTenantsList();

            this.tenants = tenants.map((tenant: Tenant) => {
                return {
                    key: new Guid(tenant.getId_asU8()).toString(),
                    text: new Guid(tenant.getId_asU8()).toString()
                };
            });
        });
    }

    detached() {
        this._getTenantsCall?.cancel();
    }
}
