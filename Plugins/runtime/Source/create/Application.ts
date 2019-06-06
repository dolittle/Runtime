/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate } from "@dolittle/tooling.common.boilerplates";
import { ICommand } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { Logger } from "@dolittle/tooling.common.logging";
import { ICanOutputMessages } from "@dolittle/tooling.common.utilities";
import { IApplicationsManager } from "../index";
/**
 * Represents an implementation of {ICommand} for creating a dolittle application
 *
 * @export
 * @class Application
 * @implements {ICommand}
 */
export class Application implements ICommand {
    
    readonly name = 'application';
    readonly description = 'Scaffolds a Dolittle application';
    readonly shortDescription = 'Scaffolds a Dolittle application';
    readonly group? = 'create';

    /**
     * Instantiates an instance of {Application}.
     * @param {IApplicationsManager} _applicationsManager
     * @param {IDependencyResolvers} _dependencyResolvers
     */
    constructor(private _applicationsManager: IApplicationsManager, private _dependencyResolvers: IDependencyResolvers, private _logger: Logger) { }
    
    async action(cwd: string, coreLanguage: string, outputter: ICanOutputMessages, commandArguments?: string[], namespace?: string) {
        let boilerplates = this._applicationsManager.boilerplatesByLanguage(coreLanguage, namespace);
        
        if (!boilerplates.length || boilerplates.length < 1) {
            this._logger.info(`No application boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `);
            outputter.warn(`No application boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `);
            return;
        }
        let boilerplate: IContentBoilerplate | null = null;
        if (boilerplates.length > 1) {
            do {
                boilerplate = <IContentBoilerplate | null> await chooseBoilerplate(boilerplates, this._dependencyResolvers); 
            } while (!boilerplate)
        }
        else boilerplate = boilerplates[0];
        
        let dependencies = boilerplate.dependencies;
        let boilerplateContext = await this._dependencyResolvers.resolve({}, dependencies, cwd, coreLanguage, commandArguments)
        this._applicationsManager.create(boilerplateContext, cwd, boilerplate as IContentBoilerplate);
    }
}
