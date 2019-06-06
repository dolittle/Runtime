/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import {IPlugin} from '@dolittle/tooling.common.plugins';
import { DefaultCommandsProvider, DefaultCommandGroupsProvider, NamespaceProvider } from './index';

/**
 * Represents an implementation of {IPlugin} that provides the dolittle runtime plugin
 *
 * @export
 * @class Plugin
 * @implements {IPlugin}
 */
export class Plugin implements IPlugin {

    readonly defaultCommandsProvider = new DefaultCommandsProvider([]);
    readonly defaultCommandGroupsProvider = new DefaultCommandGroupsProvider([]);
    readonly namespaceProvider = new NamespaceProvider([]);

}
