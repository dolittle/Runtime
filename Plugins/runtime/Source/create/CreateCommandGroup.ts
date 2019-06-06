/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { ICommandGroup, ICommand } from "@dolittle/tooling.common.commands";

/**
 * Represents an implementation of {ICommandGroup} for the dolittle create command
 *
 * @export
 * @class CreateCommandGroup
 * @implements {ICommandGroup}
 */
export class CreateCommandGroup implements ICommandGroup {

    readonly name = 'create';
    readonly description = `Commands related to scaffolding Dolittle application structures.
    
Quickly get up and running by scaffolding bounded context and application skeletons.`;
    readonly shortDescription = 'Scaffold Dolittle structures';
    
    constructor(private _commands: ICommand[]) {}
    
    get commands() { return this._commands; }
}