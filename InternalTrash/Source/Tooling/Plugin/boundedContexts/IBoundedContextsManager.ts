/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { BoundedContext } from "@dolittle/tooling.common.configurations";
import { IDependency } from "@dolittle/tooling.common.dependencies";
import { IContentBoilerplate, CreatedContentBoilerplateDetails } from "@dolittle/tooling.common.boilerplates";

/**
 * Defines a system for managing bounded contexts boilerplates
 *
 * @export
 * @interface BoundedContextsManager
 */
export interface IBoundedContextsManager {
    
    /**
     * Gets all the application boilerplates 
     *
     * @type {IContentBoilerplate[]}
     */
    getBoilerplates(): Promise<IContentBoilerplate[]>

    /**
     * Searches the file hierarchy for bounded-context.json and returns the BoundedContext
     * @param {string} startPath to search from
     * @returns {BoundedContext | null} the bounded context
     */
    getNearestBoundedContextConfig(startPath: string): Promise<BoundedContext | null>

    /**
     * Check if a bounded context configuration can be found in the given directory.
     * @param {string} folder The directory path to search
     * @returns {boolean} Whether or not the bounded context configuration was found
     */
    hasBoundedContext(folder: string): Promise<boolean>

    /**
     * Retrieves the boilerplate configurations for bounded context with the given language
     * @param {string} language 
     * @param {string} [namespace=undefined]
     * @return {IContentBoilerplate[]} The bounded context {Boilerplate} with of the given language
     */
    getBoilerplatesByLanguage(language: string, namespace?: string): Promise<IContentBoilerplate[]>

    /**
     * Gets the adornment boilerplates for a bounded context based on language and boilerplate name
     *
     * @param {string} [language=undefined] The language of the bounded context boilerplate
     * @param {string} [boilerplateName=undefined] The name of the boilerplate
     * @param {string} [namespace=undefined] The namespace of the boilerplate
     * @returns {IContentBoilerplate[]}
     */
    getAdornments(language?: string , boilerplateName?: string, namespace?: string): IContentBoilerplate[]

    /**
     * Gets the interaction adornment boilerplates for a bounded context based on language and boilerplate name
     *
     * @param {string} [language=undefined] The language of the bounded context boilerplate
     * @param {string} [boilerplateName=undefined] The name of the boilerplate
     * @param {string} [namespace=undefined] The namespace of the boilerplate
     * @returns {IContentBoilerplate[]}
     */
    getInteractionLayers(language?: string, boilerplateName?: string, namespace?: string): IContentBoilerplate[]

    /**
     * Create dependencies used for prompting the user for bounded context adornment
     *
     * @param {string} [language=undefined] The language of the bounded context boilerplate
     * @param {string} [boilerplateName=undefined] The name of the boilerplate
     * @param {string} [namespace=undefined] The namespace of the boilerplate
     * @returns {IDependency[]}
     */
    createAdornmentDependencies(language?: string, boilerplateName?: string, namespace?: string): IDependency[]
    
    /**
     * Create dependencies used for prompting the user for interaction layers
     *
     * @param {string} [language=undefined] The language of the bounded context boilerplate
     * @param {string} [boilerplateName=undefined] The name of the boilerplate
     * @param {string} [namespace=undefined] The namespace of the boilerplate
     * @returns {IDependency[]}
     */
    createInteractionDependencies(language: string, boilerplateName?: string, namespace?: string): IDependency[]

    /**
     * Creates a dolittle bounded context.
     * 
     * Interaction layers will be created as well if the dependencies are supplied in the context object.
     *
     * @param {any} context The template context
     * @param {IContentBoilerplate} boilerplate The Bounded Context Boilerplate
     * @param {string} destinationPath The absolute path of the destination of the bounded context
     * @param {string} [namespace=undefined]
     * @returns {CreatedContentBoilerplateDetails[]} Returns the created boilerplates with destination
     */
    create(context: any, boilerplate: IContentBoilerplate, destinationPath: string, namespace?: string): Promise<CreatedContentBoilerplateDetails[]>

    /**
     * Creates an interaction layer and adds it to the bounded context by finding it in the folder.
     * 
     * Use addInteractionLayerToBoundedContext if you need to add multiple interaction layers
     *
     * @param {*} context
     * @param {IContentBoilerplate} boilerplate
     * @param {string} boundedContextFolder
     * @param {string} entryPoint
     */
    addInteractionLayer(context: any, boilerplate: IContentBoilerplate, boundedContextFolder: string, entryPoint: string): Promise<void>

    /**
     * Creates an interaction layer, adds it to the bounded context and returns the bounded context object
     *
     * @param {*} context
     * @param {IContentBoilerplate} boilerplate
     * @param {BoundedContext} boundedContext
     * @param {string} entryPoint
     * @returns {BoundedContext}
     */
    addInteractionLayerToBoundedContext(context: any, boilerplate: IContentBoilerplate, boundedContext: BoundedContext, entryPoint: string): Promise<BoundedContext>
    
}