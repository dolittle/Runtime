// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { PLATFORM } from 'aurelia-pal';
import { Router, RouterConfiguration } from 'aurelia-router';

export class App {
    router: any;

    configureRouter(config: RouterConfiguration, router: Router) {
        config.options.pushState = true;
        config.map([{ route: '', name: 'Index', moduleId: PLATFORM.moduleName('index') }]);

        this.router = router;
    }
}
