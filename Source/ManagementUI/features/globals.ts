// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

export class Globals {
    currentTenant: Guid;

    constructor() {
        this.currentTenant = Guid.empty;
    }
}
