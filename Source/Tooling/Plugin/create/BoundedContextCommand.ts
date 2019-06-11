/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate, IScriptRunner } from "@dolittle/tooling.common.boilerplates";
import { Command } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { ICanOutputMessages, NullMessageOutputter, IBusyIndicator, NullBusyIndicator } from "@dolittle/tooling.common.utilities";
import { Logger } from "@dolittle/tooling.common.logging";

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
    constructor(private _boundedContextsManager: IBoundedContextsManager, private _dependencyResolvers: IDependencyResolvers, private _scriptRunner: IScriptRunner, private _logger: Logger) { 
        super(name, description);
    }
    
    async action(cwd: string, coreLanguage: string, commandArguments?: string[], options?: Map<string, string>, namespace?: string, 
                outputter: ICanOutputMessages = new NullMessageOutputter(), busyIndicator: IBusyIndicator = new NullBusyIndicator()) {
        let boilerplates = this._boundedContextsManager.boilerplatesByLanguage(coreLanguage, namespace);
        if (!boilerplates.length || boilerplates.length < 1) {
            this._logger.warn(`No bounded context boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `);
            outputter.warn(`No bounded context boilerplates found for language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''} `);
            return;
        }
        let boilerplate: IContentBoilerplate | null = null;
        if (boilerplates.length > 1) {
            do {
                boilerplate = <IContentBoilerplate | null> await chooseBoilerplate(boilerplates, this._dependencyResolvers);
            } while(!boilerplate)

        }
        else boilerplate = boilerplates[0];

        let dependencies = [
                ...boilerplate.dependencies, 
                ...this._boundedContextsManager.createAdornmentDependencies(boilerplate.language, boilerplate.name, namespace),
                ...this._boundedContextsManager.createInteractionDependencies(boilerplate.language, boilerplate.name, namespace)
            ];

        let boilerplateContext = await this._dependencyResolvers.resolve({}, dependencies, cwd, coreLanguage, commandArguments);

        let createdDetails = this._boundedContextsManager.create(boilerplateContext, boilerplate, cwd, namespace);
       
        createdDetails.forEach(_ => {
            let scripts: any[] = [];
            scripts.push(..._.boilerplate.scripts.creation);
            
            let cwd = _.destination;
            if (scripts && scripts.length > 0) {
                outputter.print(`Running creation scripts for boilerplate ${_.boilerplate.name}. This might take a while`);
                this._scriptRunner.runSync(scripts, cwd, outputter.warn, outputter.print, (error) => {}); 
            }
        });
    }
    
    getAllDependencies(cwd: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, string>, namespace?: string) {
        let boilerplate = this._boundedContextsManager.boilerplatesByLanguage(coreLanguage, namespace)[0];
        let dependencies = boilerplate? boilerplate.dependencies : [];
        return this.dependencies.concat(dependencies);
    }
}
