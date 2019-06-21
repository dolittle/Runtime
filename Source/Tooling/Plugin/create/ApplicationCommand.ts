/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate } from "@dolittle/tooling.common.boilerplates";
import { Command } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { Logger } from "@dolittle/tooling.common.logging";
import { ICanOutputMessages, NullMessageOutputter, IBusyIndicator, NullBusyIndicator } from "@dolittle/tooling.common.utilities";
import { IApplicationsManager } from "../index";

const name = 'application';
const description = 'Scaffolds a Dolittle application';

/**
 * Represents an implementation of {ICommand} for creating a dolittle application
 *
 * @export
 * @class ApplicationCommand
 * @extends {Command}
 */
export class ApplicationCommand extends Command {
    
    /**
     * Instantiates an instance of {ApplicationCommand}.
     * @param {IApplicationsManager} _applicationsManager
     * @param {IDependencyResolvers} _dependencyResolvers
     */
    constructor(private _applicationsManager: IApplicationsManager, private _dependencyResolvers: IDependencyResolvers, private _logger: Logger) {
        super(name, description, true);
    }
    
    async action(dependencyResolvers: IDependencyResolvers, cwd: string, coreLanguage: string, commandArguments?: string[], options?: Map<string, any>, namespace?: string, 
                outputter: ICanOutputMessages = new NullMessageOutputter(), busyIndicator: IBusyIndicator = new NullBusyIndicator()) {
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

    getAllDependencies(cwd: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, any>, namespace?: string) {
        let boilerplate = this._applicationsManager.boilerplatesByLanguage(coreLanguage, namespace)[0];
        let dependencies = boilerplate? boilerplate.dependencies : [];
        return this.dependencies.concat(dependencies);
    }
}
