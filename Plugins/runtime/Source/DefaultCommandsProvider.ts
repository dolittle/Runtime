/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { ICanProvideDefaultCommands, ICommand } from "@dolittle/tooling.common.commands";

/**
 * Represents an implementation of {ICanProvideDefaultCommands} for providing default commands
 *
 * @export
 * @class DefaultCommandsProvider
 * @implements {ICanProvideDefaultCommands}
 */
export class DefaultCommandsProvider implements ICanProvideDefaultCommands {

    constructor(private _commandGroups: ICommand[]) {}

    provide(): ICommand[] {
        return this._commandGroups;
    }

}