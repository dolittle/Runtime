/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { DiscoverableCommandGroup, ICommand } from "@dolittle/tooling.common.commands";
import { ITemplatesBoilerplates, ITemplate } from "@dolittle/tooling.common.boilerplates";
import { groupBy } from "@dolittle/tooling.common.utilities";
import { AddCommand, IBoundedContextsManager } from "../index";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { Folders } from "@dolittle/tooling.common.files";

const name = 'add';
const description = `Adds basic building blocks to an existing bounded context.

What can be added to a bounded context is based on the boilerplates available on the local system.`;

/**
 * Represents an implementation of {ICommandGroup} for the command group related to adding templates to bounded contexts
 *
 * @export
 * @class AddCommandGroup
 * @extends {DiscoverableCommandGroup}
 */
export class AddCommandGroup extends DiscoverableCommandGroup {

    private _commands!: ICommand[];

    constructor(private _templatesBoilerplates: ITemplatesBoilerplates, private _dependencyResolvers: IDependencyResolvers, 
                private _boundedContextsManager: IBoundedContextsManager, private _folders: Folders, private _dolittleConfig: any) {
        super(name, description, 'Adds basic building blocks to an existing bounded context');
    }

    get commands() {
        if (!this._commands) this.loadCommands();
        return this._commands;
    }
    
    loadCommands(): void {
        this._commands = [];
        let boilerplates = this._templatesBoilerplates.boilerplates;
        boilerplates.forEach(boilerplate => {
            let templates: ITemplate[] = [];
            templates.push(...boilerplate.templates);
            let templateGroups: {[key: string]: ITemplate[]} = groupBy('type')(templates);
            Object.keys(templateGroups).forEach(type => {
                let templates = templateGroups[type];
                this._commands.push(new AddCommand(boilerplate, type, templates, this._templatesBoilerplates, this._dependencyResolvers, this._boundedContextsManager, this._folders, this._dolittleConfig));
            })
        });
    }
}
