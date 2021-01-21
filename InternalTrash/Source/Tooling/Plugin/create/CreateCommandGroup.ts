/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { CommandGroup, ICommand } from "@dolittle/tooling.common.commands";

const name = 'create';

const description = `Commands related to scaffolding Dolittle application structures.
    
Quickly get up and running by scaffolding bounded context and application skeletons.`;

const shortDescription = 'Scaffold Dolittle structures';
/**
 * Represents an implementation of {ICommandGroup} for the dolittle create command
 *
 * @export
 * @class CreateCommandGroup
 * @extends {CommandGroup}
 */
export class CreateCommandGroup extends CommandGroup {

    /**
     * Instantiates an instance of {CreateCommandGroup}.
     * @param {ICommand[]} commands
     */
    constructor(commands: ICommand[]) {
        super(name, commands, description, true, shortDescription);
    }
}