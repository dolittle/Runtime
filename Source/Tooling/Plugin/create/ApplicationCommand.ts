/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate } from "@dolittle/tooling.common.boilerplates";
import { Command } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { ILoggers } from "@dolittle/tooling.common.logging";
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
    constructor(private _applicationsManager: IApplicationsManager, private _dependencyResolvers: IDependencyResolvers, private _logger: ILoggers) {
        super(name, description, true);
    }
    
    async action(dependencyResolvers: IDependencyResolvers, cwd: string, coreLanguage: string, commandArguments?: string[], options?: Map<string, any>, namespace?: string, 
                outputter: ICanOutputMessages = new NullMessageOutputter(), busyIndicator: IBusyIndicator = new NullBusyIndicator()) {
        
        let boilerplate = await this.chooseABoilerplate(dependencyResolvers, outputter, coreLanguage, namespace);
        if (boilerplate === null) return;
        let dependencies = boilerplate.dependencies;
        let boilerplateContext = await dependencyResolvers.resolve({}, dependencies, cwd, coreLanguage, commandArguments, options)

        await this._applicationsManager.create(boilerplateContext, cwd, boilerplate as IContentBoilerplate);
    }

    getAllDependencies(cwd: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, any>, namespace?: string) {
        let boilerplate = this._applicationsManager.boilerplatesByLanguage(coreLanguage, namespace)[0];
        let dependencies = boilerplate? boilerplate.dependencies : [];
        return this.dependencies.concat(dependencies);
    }

    private async chooseABoilerplate(dependencyResolvers: IDependencyResolvers, outputter: ICanOutputMessages, coreLanguage: string, namespace?: string) {
        let boilerplates = this._applicationsManager.boilerplatesByLanguage(coreLanguage, namespace);
        
        if (!boilerplates.length || boilerplates.length === 0) {
            let message = `No application boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `;
            this._logger.info(message);
            outputter.warn(message);
            return null;
        }
        let boilerplate: IContentBoilerplate | null = boilerplates[0];

        if (boilerplates.length > 1) {
            do {
                boilerplate = <IContentBoilerplate | null> await chooseBoilerplate(boilerplates, dependencyResolvers); 
            } while (!boilerplate)
        }
        return boilerplate;
    }
}
