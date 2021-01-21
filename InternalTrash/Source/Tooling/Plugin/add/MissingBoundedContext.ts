/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import {Exception} from '@dolittle/tooling.common.utilities';

/**
 * The exception that gets thrown when bounded context cannot be found 
 *
 * @export
 * @class MissingBoundedContext
 * @extends {Exception}
 */
export class MissingBoundedContext extends Exception {

    /**
     * Instantiates an instance of {MissingBoundedContext}.
     * @param {string} cwd
     */
    constructor(cwd: string) {
        super(`Could not find bounded context searching from path '${cwd}'`);
    }
}
