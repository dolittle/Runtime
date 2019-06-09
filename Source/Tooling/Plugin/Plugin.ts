/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import {IPlugin} from '@dolittle/tooling.common.plugins';
import { ICanProvideDefaultCommands, ICanProvideDefaultCommandGroups, ICanProvideNamespaces } from '@dolittle/tooling.common.commands';
import { defaultCommandsProvider, defaultCommandGroupsProvider, namespaceProvider } from './index';

/**
 * Represents an implementation of {IPlugin} that provides the dolittle runtime plugin
 *
 * @class Plugin
 * @implements {IPlugin}
 */
class Plugin implements IPlugin {

    constructor(defaultCommandsProvider: ICanProvideDefaultCommands, defaultCommandGroupsProvider: ICanProvideDefaultCommandGroups, namespaceProvider: ICanProvideNamespaces) {
        this.defaultCommandsProvider = defaultCommandsProvider;
        this.defaultCommandGroupsProvider = defaultCommandGroupsProvider;
        this.namespaceProvider = namespaceProvider;
    }

    readonly defaultCommandsProvider: ICanProvideDefaultCommands;

    readonly defaultCommandGroupsProvider: ICanProvideDefaultCommandGroups;

    readonly namespaceProvider: ICanProvideNamespaces;

}

export let plugin = new Plugin(defaultCommandsProvider, defaultCommandGroupsProvider, namespaceProvider);