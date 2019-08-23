/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Application } from '@dolittle/tooling.common.configurations';
import { IContentBoilerplate, CreatedContentBoilerplateDetails } from "@dolittle/tooling.common.boilerplates";

/**
 * Defines a system that can manage the application boilerplates
 *
 * @export
 * @interface IApplicationsManager
 */
export interface IApplicationsManager {

    /**
     * Gets all the application boilerplates 
     *
     * @type {IContentBoilerplate[]}
     */
    boilerplates: IContentBoilerplate[]

    /**
     * Gets the application configuration from the given folder
     * @param {string} folder path 
     * @returns {Promise<Application | null>} application config or null if not found
     */
    getApplicationFrom(folder: string): Promise<Application | null>

    /**
     * Check if an application has been setup in the given folder.
     * @param {string} folder path
     * @returns {Promise<boolean>} whether or not the application configuration is set up
     */
    hasApplication(folder: string): Promise<boolean>

    /**
     * Retrieves the boilerplate configurations for application with the given language
     * @param {string} language 
     * @param {string} [namespace=undefined]
     * @return {IContentBoilerplate[]} The application {Boilerplate} with of the given language
     */
    boilerplatesByLanguage(language: string, namespace?: string): IContentBoilerplate[]

    /**
     * Creates a dolittle application
     *
     * @param {any} context The template context 
     * @param {string} destinationPath The absolute path of the destination of the application
     * @param {IContentBoilerplate} boilerplate The boilerplate to create the application from
     * @returns {Promise<CreatedContentBoilerplateDetails[]>}
     */
    create(context: any, destinationPath: string, boilerplate: IContentBoilerplate): Promise<CreatedContentBoilerplateDetails[]>
}