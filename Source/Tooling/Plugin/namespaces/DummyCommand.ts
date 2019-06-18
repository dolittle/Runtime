/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { Command } from "@dolittle/tooling.common.commands";
import { ICanOutputMessages, IBusyIndicator } from "@dolittle/tooling.common.utilities";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";

export class DummyCommand extends Command {
    
    constructor() {
        super('dummy', 'something')
    }
    
    async action(dependencyResolvers: IDependencyResolvers, currentWorkingDirectory: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, any>, namespace?: string, outputter?: ICanOutputMessages, busyIndicator?: IBusyIndicator) {
        console.log('Something');
    }
    
    getAllDependencies(currentWorkingDirectory: string, coreLanguage: string, commandArguments?: string[] | undefined, commandOptions?: Map<string, any> | undefined, namespace?: string | undefined): import("@dolittle/tooling.common.dependencies").IDependency[] {
        return this.dependencies;
    }
    
}