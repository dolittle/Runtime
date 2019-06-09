/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { ICanProvideDefaultCommandGroups, ICommandGroup } from "@dolittle/tooling.common.commands";

/**
 * Represents an implementation of {ICanProvideDefaultCommandGroups} for providing the command groups of the runtime plugin
 *
 * @export
 * @class DefaultCommandGroupsProvider
 * @implements {ICanProvideDefaultCommandGroups}
 */
export class DefaultCommandGroupsProvider implements ICanProvideDefaultCommandGroups {

    constructor(private _commandGroups: ICommandGroup[]) {}

    provide(): ICommandGroup[] {
        return this._commandGroups;
    }

}