/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { DiscoverableCommandGroup, ICommand } from "@dolittle/tooling.common.commands";
import { ITemplatesBoilerplates, ITemplate, IBoilerplatesLoader } from "@dolittle/tooling.common.boilerplates";
import { groupBy } from "@dolittle/tooling.common.utilities";
import { AddCommand, IBoundedContextsManager } from "../internal";
import { IFolders } from "@dolittle/tooling.common.files";

const name = 'add';
const description = `Adds basic building blocks to an existing bounded context.

What can be added to a bounded context is based on the boilerplates available on the local system.`;

export class AddCommandGroup extends DiscoverableCommandGroup {

    private _commands!: ICommand[];

    constructor(private _boilerplatesLoader: IBoilerplatesLoader, private _templatesBoilerplates: ITemplatesBoilerplates, 
                private _boundedContextsManager: IBoundedContextsManager, private _folders: IFolders, private _dolittleConfig: any) {
        super(name, description, true, 'Adds basic building blocks to an existing bounded context');
    }

    async getCommands() {
        if (!this._commands) await this.loadCommands();
        return this._commands;
    }
    
    async loadCommands() {
        this._commands = [];
        if (this._boilerplatesLoader.needsReload) await this._boilerplatesLoader.load()
        let boilerplates = this._templatesBoilerplates.boilerplates;
        boilerplates.forEach(boilerplate => {
            let templates: ITemplate[] = [];
            templates.push(...boilerplate.templates);
            let templateGroups: {[key: string]: ITemplate[]} = groupBy('type')(templates);
            Object.keys(templateGroups).forEach(type => {
                let templates = templateGroups[type];
                this._commands.push(new AddCommand(boilerplate, type, templates, this._templatesBoilerplates, this._boundedContextsManager, this._folders, this._dolittleConfig));
            })
        });
    }
}
