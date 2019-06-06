/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { contentBoilerplates, scriptRunner } from "@dolittle/tooling.common.boilerplates";
import { fileSystem, folders } from "@dolittle/tooling.common.files";
import { logger } from "@dolittle/tooling.common.logging";
import { dependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { ICommandGroup, ICanProvideDefaultCommandGroups, ICanProvideDefaultCommands, ICanProvideNamespaces } from "@dolittle/tooling.common.commands";
import { CreateCommandGroup, Application, ApplicationsManager, DefaultCommandGroupsProvider, DefaultCommandsProvider, NamespaceProvider, BoundedContext, BoundedContextsManager } from "./index";

let applicationsManager = new ApplicationsManager(contentBoilerplates, fileSystem, logger);
let boundedContextsManager = new BoundedContextsManager(contentBoilerplates, applicationsManager, folders, fileSystem, logger);
let createCommandGroup: ICommandGroup = new CreateCommandGroup([
    new Application(applicationsManager, dependencyResolvers, logger),
    new BoundedContext(boundedContextsManager, dependencyResolvers, scriptRunner, logger)
]);

export let defaultCommandGroupsProvider: ICanProvideDefaultCommandGroups = new DefaultCommandGroupsProvider([createCommandGroup]);

export let defaultCommandsProvider: ICanProvideDefaultCommands = new DefaultCommandsProvider([]);

export let defaultNamespaceProvider: ICanProvideNamespaces = new NamespaceProvider([]);