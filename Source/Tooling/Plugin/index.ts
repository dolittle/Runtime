/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { contentBoilerplates, boilerplatesLoader, templatesBoilerplates, scriptRunner } from "@dolittle/tooling.common.boilerplates";
import { dolittleConfig } from "@dolittle/tooling.common.configurations";
import { fileSystem, folders } from "@dolittle/tooling.common.files";
import { loggers } from "@dolittle/tooling.common.logging";
import { Plugin } from "@dolittle/tooling.common.plugins";
import { 
    ApplicationsManager, BoundedContextsManager, CommandGroupsProvider, AddCommandGroup,
    CreateCommandGroup, ApplicationCommand, BoundedContextCommand, CommandsProvider, NamespaceProvider, RuntimeNamespace, AddFeatureCommand
} from "./internal";

let applicationsManager = new ApplicationsManager(contentBoilerplates, boilerplatesLoader, fileSystem, loggers);
let boundedContextsManager = new BoundedContextsManager(contentBoilerplates, boilerplatesLoader, applicationsManager, folders, fileSystem, loggers);

let commandGroupsProvider = new CommandGroupsProvider([
    new AddCommandGroup(boilerplatesLoader, templatesBoilerplates, boundedContextsManager, folders, dolittleConfig),
    new CreateCommandGroup([
        new ApplicationCommand(applicationsManager, loggers),
        new BoundedContextCommand(boundedContextsManager, scriptRunner, loggers)
    ])
]);

let commandsProvider = new CommandsProvider([new AddFeatureCommand(boundedContextsManager, fileSystem, loggers)]);
let namespaceProvider = new NamespaceProvider([new RuntimeNamespace()]);

export let plugin = new Plugin(commandsProvider, commandGroupsProvider, namespaceProvider);
