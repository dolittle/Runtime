/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { ICanProvideCommandGroups, ICommandGroup } from "@dolittle/tooling.common.commands";

/**
 * Represents an implementation of {ICanProvideCommandGroups} for providing the command groups of the runtime plugin
 *
 * @export
 * @class CommandGroupsProvider
 * @implements {ICanProvideCommandGroups}
 */
export class CommandGroupsProvider implements ICanProvideCommandGroups {

    constructor(private _commandGroups: ICommandGroup[]) {}

    provide(): ICommandGroup[] {
        return this._commandGroups;
    }

}
