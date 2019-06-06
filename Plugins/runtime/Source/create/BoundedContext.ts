/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { chooseBoilerplate, IContentBoilerplate, IScriptRunner, Script } from "@dolittle/tooling.common.boilerplates";
import { ICommand } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { ICanOutputMessages } from "@dolittle/tooling.common.utilities";
import { Logger } from "@dolittle/tooling.common.logging";

import { IBoundedContextsManager } from "../index";

/**
 * Represents an implementation of {ICommand} for creating a dolittle bounded context
 *
 * @export
 * @class BoundedContext
 * @implements {ICommand}
 */
export class BoundedContext implements ICommand {
    
    readonly name = 'boundedcontext';
    readonly description = 'Scaffolds a Dolittle bounded context';
    readonly shortDescription = 'Scaffolds a Dolittle bounded context';
    readonly group? = 'create';

    /**
     * Instantiates an instance of {BoundedContext}.
     * @param {IBoundedContextsManager} _boundedContextsManager
     * @param {IDependencyResolvers} _dependencyResolvers
     */
    constructor(private _boundedContextsManager: IBoundedContextsManager, private _dependencyResolvers: IDependencyResolvers, private _scriptRunner: IScriptRunner, private _logger: Logger) { }
    
    async action(cwd: string, coreLanguage: string, outputter: ICanOutputMessages, commandArguments?: string[], namespace?: string) {;
        
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
}
