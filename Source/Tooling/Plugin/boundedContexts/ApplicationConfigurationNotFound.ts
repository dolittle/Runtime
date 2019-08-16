/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { Exception } from "@dolittle/tooling.common.utilities";

/**
 * The exception that gets throw when an application configuration was expected but not found
 *
 * @export
 * @class ApplicationConfigurationNotFound
 * @extends {Exception}
 */
export class ApplicationConfigurationNotFound extends Exception {
    
    /**
     * Instantiates an instance of {ApplicationConfigurationNotFound}.
     * @param {string} fromPath 
     */
    constructor(fromPath: string) {
        super(`Could not find application configuration when searching from '${fromPath}'`);
    }
}
