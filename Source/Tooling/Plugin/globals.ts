/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { contentBoilerplates, scriptRunner, templatesBoilerplates, boilerplatesLoader } from "@dolittle/tooling.common.boilerplates";
import { fileSystem, folders } from "@dolittle/tooling.common.files";
import { loggers } from "@dolittle/tooling.common.logging";
import { dependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { CreateCommandGroup, ApplicationCommand, ApplicationsManager, BoundedContextsManager, BoundedContextCommand, DefaultCommandGroupsProvider, DefaultCommandsProvider, NamespaceProvider, AddCommandGroup, RuntimeNamespace } from "./index";
import { dolittleConfig } from "@dolittle/tooling.common.configurations";

let applicationsManager = new ApplicationsManager(contentBoilerplates, fileSystem, loggers);
let boundedContextsManager = new BoundedContextsManager(contentBoilerplates, applicationsManager, folders, fileSystem, loggers);

export let defaultCommandGroupsProvider = new DefaultCommandGroupsProvider([
    new AddCommandGroup(boilerplatesLoader, templatesBoilerplates, boundedContextsManager, folders, dolittleConfig),
    new CreateCommandGroup([
        new ApplicationCommand(applicationsManager, dependencyResolvers, loggers),
        new BoundedContextCommand(boundedContextsManager, scriptRunner, loggers)
    ])
]);

export let defaultCommandsProvider = new DefaultCommandsProvider([]);
export let namespaceProvider = new NamespaceProvider([new RuntimeNamespace()]);
