/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { IPlugin } from "@dolittle/tooling.common.plugins";
import { Plugin } from "./Plugin";

export * from './Plugin';
export * from './DefaultCommandGroupsProvider';
export * from './DefaultCommandsProvider';
export * from './NamespaceProvider';

export * from './applications/index';
export * from './boundedContexts/index';
export * from './create/index';

export const plugin: IPlugin = new Plugin()