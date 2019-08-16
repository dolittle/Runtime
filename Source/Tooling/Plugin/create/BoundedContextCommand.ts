/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate, IScriptRunner, CreatedContentBoilerplateDetails } from "@dolittle/tooling.common.boilerplates";
import { Command } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { ICanOutputMessages, NullMessageOutputter, IBusyIndicator, NullBusyIndicator } from "@dolittle/tooling.common.utilities";
import { ILoggers } from "@dolittle/tooling.common.logging";

import { IBoundedContextsManager } from "../index";

const name = 'boundedcontext';
const description = 'Scaffolds a Dolittle bounded context';

/**
 * Represents an implementation of {ICommand} for creating a dolittle bounded context
 *
 * @export
 * @class BoundedContextCommand
 * @extends {Command}
 */
export class BoundedContextCommand extends Command {
    
    /**
     * Instantiates an instance of {BoundedContextCommand}.
     * @param {IBoundedContextsManager} _boundedContextsManager
     * @param {IDependencyResolvers} _dependencyResolvers
     */
    constructor(private _boundedContextsManager: IBoundedContextsManager, private _scriptRunner: IScriptRunner, private _logger: ILoggers) { 
        super(name, description, true);
    }
    
    async action(dependencyResolvers: IDependencyResolvers, cwd: string, coreLanguage: string, commandArguments?: string[], options?: Map<string, any>, namespace?: string, 
                outputter: ICanOutputMessages = new NullMessageOutputter(), busyIndicator: IBusyIndicator = new NullBusyIndicator()) {
        let boilerplate = await this.chooseABoilerplate(dependencyResolvers, outputter, coreLanguage, namespace);
        if (boilerplate === null) return;

        let dependencies = [
                ...boilerplate.dependencies, 
                ...this._boundedContextsManager.createAdornmentDependencies(boilerplate.language, boilerplate.name, namespace),
                ...this._boundedContextsManager.createInteractionDependencies(boilerplate.language, boilerplate.name, namespace)
            ];

        let boilerplateContext = await dependencyResolvers.resolve({}, dependencies, cwd, coreLanguage, commandArguments);

        let createdDetails = await this._boundedContextsManager.create(boilerplateContext, boilerplate, cwd, namespace);
       
        this.runCreationScriptsSync(createdDetails, outputter);
    }
    
    getAllDependencies(cwd: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, any>, namespace?: string) {
        let boilerplate = this._boundedContextsManager.boilerplatesByLanguage(coreLanguage, namespace)[0];
        let dependencies = boilerplate? boilerplate.dependencies : [];
        return this.dependencies.concat(dependencies);
    }

    private runCreationScriptsSync(createdDetails: CreatedContentBoilerplateDetails[], outputter: ICanOutputMessages) {
        createdDetails.forEach(_ => {
            let scripts = _.boilerplate.scripts.creation;
            
            let cwd = _.destination;
            if (scripts && scripts.length > 0) {
                outputter.print(`Running creation scripts for boilerplate ${_.boilerplate.name}. This might take a while`);
                this._scriptRunner.runSync(scripts, cwd, outputter.warn, outputter.print, (error) => {}); 
            }
        });
    }

    private async chooseABoilerplate(dependencyResolvers: IDependencyResolvers, outputter: ICanOutputMessages, coreLanguage: string, namespace?: string) {
        let boilerplates = this._boundedContextsManager.boilerplatesByLanguage(coreLanguage, namespace);
        if (!boilerplates.length || boilerplates.length < 1) {
            let message = `No bounded context boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `;
            this._logger.warn(message);
            outputter.warn(message);
            return null;
        }
        let boilerplate: IContentBoilerplate | null = boilerplates[0];
        if (boilerplates.length > 1) {
            do {
                boilerplate = <IContentBoilerplate | null> await chooseBoilerplate(boilerplates, dependencyResolvers);
            } while(!boilerplate)
        }
        return boilerplate;
    }
}
