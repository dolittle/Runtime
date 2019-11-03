/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { Command, CommandContext, IFailedCommandOutputter, CommandFailed } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers, PromptDependency, IsNotEmpty, argumentUserInputType,  } from "@dolittle/tooling.common.dependencies";
import { ILoggers } from "@dolittle/tooling.common.logging";
import { ICanOutputMessages, NullMessageOutputter, IBusyIndicator, NullBusyIndicator, Exception, areas, determineDestination } from "@dolittle/tooling.common.utilities";
import { IBoundedContextsManager } from "../internal";
import { dolittleConfig } from "@dolittle/tooling.common.configurations";
import { IFileSystem } from "@dolittle/tooling.common.files";

const name = 'add-feature';
const description = 'Adds a feature across areas in a bounded context';

let featureDependency = new PromptDependency(
    'feature',
    `The feature to add across areas. A single string which can consist of several features separated by a dot '.'. 
For example Feature1.SubFeature1.SubFeature2`,
    [new IsNotEmpty()],
    argumentUserInputType,
    'The feature to add'
);

export class AddFeatureCommand extends Command {
    
    constructor(private _boundedContextsManager: IBoundedContextsManager, private _fileSystem: IFileSystem, private _logger: ILoggers) {
        super(name, description, false, 'Add a feature', [featureDependency] );
    }
    
    async onAction(commandContext: CommandContext, dependencyResolvers: IDependencyResolvers, failedCommandOutputter: IFailedCommandOutputter, outputter: ICanOutputMessages, busyIndicator: IBusyIndicator) {
        let boundedContext = await this._boundedContextsManager.getNearestBoundedContextConfig(commandContext.currentWorkingDirectory)
        if (!boundedContext) throw new Exception(`Could not add feature because you're not in the context of a bounded context. ${commandContext.currentWorkingDirectory} is not the root of a bounded context`);
        
        let context = await dependencyResolvers.resolve({}, this.dependencies, [], commandContext.currentWorkingDirectory, commandContext.coreLanguage);
        let feature = context.feature as string;

        for (let area of areas) {
            let destination = determineDestination(area, commandContext.coreLanguage, `${feature}.`, commandContext.currentWorkingDirectory, boundedContext!.path, dolittleConfig);
            await this._fileSystem.ensureDirectory(destination.destination);
        }
    }

}
