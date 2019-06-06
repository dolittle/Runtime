/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { IPlugin } from "@dolittle/tooling.common.plugins";
import { Plugin } from "./Plugin";
import {defaultCommandGroupsProvider, defaultCommandsProvider, namespaceProvider} from './globals';

export * from './Plugin';
export * from './DefaultCommandGroupsProvider';
export * from './DefaultCommandsProvider';
export * from './NamespaceProvider';

export * from './applications/index';
export * from './boundedContexts/index';
export * from './create/index';
export * from './globals';

export const plugin: IPlugin = new Plugin(defaultCommandsProvider, defaultCommandGroupsProvider, namespaceProvider);