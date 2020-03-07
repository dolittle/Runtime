// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Aurelia } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import { PLATFORM } from 'aurelia-pal';
import 'aurelia-polyfills';
import environment from './environment';
import { Globals } from './globals';

export function configure(aurelia: Aurelia) {
    aurelia.use
        .standardConfiguration()
        .feature(PLATFORM.moduleName('resources/index'))
        .plugin(PLATFORM.moduleName('aurelia-animator-css'))
        .plugin(PLATFORM.moduleName('@dunite/au-office-ui'));

    const globals = new Globals()
    aurelia.container.registerInstance<Globals>(Globals, globals);

    aurelia.container.registerHandler<HttpClient>(HttpClient, () => {
        const client = new HttpClient();
        client.configure(config => {
            config.withInterceptor({
                request(request) {
                    request.headers.append('Tenant-ID', globals.currentTenant.toString());
                    return request;
                }
            })
        });
        return client;
    });

    aurelia.use.developmentLogging(environment.debug ? 'debug' : 'warn');

    if (environment.testing) {
        aurelia.use.plugin(PLATFORM.moduleName('aurelia-testing'));
    }

    aurelia.start().then(() => aurelia.setRoot(PLATFORM.moduleName('App')));
}
