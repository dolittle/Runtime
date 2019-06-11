/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { contentBoilerplates, scriptRunner } from "@dolittle/tooling.common.boilerplates";
import { fileSystem, folders } from "@dolittle/tooling.common.files";
import { logger } from "@dolittle/tooling.common.logging";
import { dependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { CreateCommandGroup, ApplicationCommand, ApplicationsManager, BoundedContextsManager, BoundedContextCommand, DefaultCommandGroupsProvider, DefaultCommandsProvider, NamespaceProvider } from "./index";

let applicationsManager = new ApplicationsManager(contentBoilerplates, fileSystem, logger);
let boundedContextsManager = new BoundedContextsManager(contentBoilerplates, applicationsManager, folders, fileSystem, logger);

export let defaultCommandGroupsProvider = new DefaultCommandGroupsProvider([
    new CreateCommandGroup([
        new ApplicationCommand(applicationsManager, dependencyResolvers, logger),
        new BoundedContextCommand(boundedContextsManager, dependencyResolvers, scriptRunner, logger)
    ])
]);

export let defaultCommandsProvider = new DefaultCommandsProvider([]);
export let namespaceProvider = new NamespaceProvider([]);