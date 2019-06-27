/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { BoundedContext, boundedContextFileName, Core, InteractionLayer, Resources } from '@dolittle/tooling.common.configurations';
import { PromptDependency, chooseOneUserInputType } from '@dolittle/tooling.common.dependencies';
import { Folders, getFileDirPath, FileSystem} from '@dolittle/tooling.common.files';
import { groupBy } from '@dolittle/tooling.common.utilities';
import { Logger } from '@dolittle/tooling.common.logging';
import path from 'path';
import { ContentBoilerplate, IContentBoilerplates, IContentBoilerplate, CreatedContentBoilerplateDetails } from '@dolittle/tooling.common.boilerplates';
import { IBoundedContextsManager, IApplicationsManager, ApplicationConfigurationNotFound } from '../index';


export const boundedContextBoilerplateType = 'boundedContext';

const boundedContextAdornmentDependencyName = 'boundedContextAdornment'

/**
 * Represents an implementation of {IBoundedContextsManager}
 *
 * @export
 * @class BoundedContextsManager
 */
export class BoundedContextsManager implements IBoundedContextsManager {

    /**
     *Creates an instance of BoundedContextsManager.
     * @param {IContentBoilerplates} _boilerplates
     * @param {IBoilerplatesCreator} _boilerplatesCreator
     * @param {IApplicationsManager} _applicationsManager
     * @param {Folders} _folders
     * @param {FileSystem} _fileSystem
     * @param {Logger} _logger
     */
    constructor(private _boilerplates: IContentBoilerplates, private _applicationsManager: IApplicationsManager,
        private _folders: Folders, private _filesystem: FileSystem, private _logger: Logger) {}
    
    get boilerplates() {
        return this._boilerplates.byType(boundedContextBoilerplateType) as IContentBoilerplate[];
    }

    getNearestBoundedContextConfig(startPath: string) {
        let regex =  new RegExp('\\b'+boundedContextFileName+'\\b');
        const boundedContextConfigPath = this._folders.getNearestFileSearchingUpwards(startPath, regex);
        if (boundedContextConfigPath === undefined || boundedContextConfigPath === '') return null;
        this._logger.info(`Found bounded context configuration at path '${boundedContextConfigPath}'`);

        let boundedContextObj = JSON.parse(this._filesystem.readFileSync(boundedContextConfigPath, 'utf8'));
        let boundedContext = BoundedContext.fromJson(boundedContextObj, boundedContextConfigPath);
        
        return boundedContext;
    }

    hasBoundedContext(folder: string) {
        const filePath = path.join(folder, boundedContextFileName);
        return this._filesystem.existsSync(filePath);
    }

    boilerplatesByLanguage(language: string, namespace?: string) {
        let boilerplates = this.boilerplates;
        return boilerplates.filter( _ => {
            if (namespace && _.namespace) return _.namespace === namespace && _.language === language;
            return _.language && language; 
        })
    }

    getAdornments(language?: string, boilerplateName?: string, namespace?: string) {
        let adornments = this._boilerplates.adornmentsFor(boundedContextBoilerplateType, language, boilerplateName, namespace)
        return adornments.filter(_ => _.type === 'adornment');
    }
    
    getInteractionLayers(language?: string, boilerplateName?: string, namespace?: string) {
        let adornments = this._boilerplates.adornmentsFor(boundedContextBoilerplateType, language, boilerplateName, namespace)
        return adornments.filter(_ => _.type === 'interaction');
     }
    
    createAdornmentDependencies(language?: string, boilerplateName?: string, namespace?: string) {
        let adornments = this.getAdornments(language, boilerplateName, namespace);
        if (adornments.length === 0) return [];
        let boundedContextAdornment = new PromptDependency(
            `${boundedContextAdornmentDependencyName}`,
            `Choose bounded context adornment`,
            chooseOneUserInputType,
            `Choose bounded context adornment`,
            false,
            adornments.map(_ => _.name).concat('None')
        );

        return [boundedContextAdornment];
    }
    
    createInteractionDependencies(language?: string, boilerplateName?: string, namespace?: string) {
        let interactionLayers = this.getInteractionLayers(language, boilerplateName, namespace);
        let interactionLayerTypes = groupBy('target')(interactionLayers);
        return Object.keys(interactionLayerTypes)
            .map(target => new PromptDependency(
                `interaction${target}`,
                `Choose ${target} interaction layer`,
                chooseOneUserInputType,
                `Choose ${target} interaction layer`,
                false,
                interactionLayers.map(_ => _.name).concat('None')
            ));
    }

    create(context: any, boilerplate: ContentBoilerplate, destinationPath: string, namespace?: string) {

        let createdDetails: CreatedContentBoilerplateDetails[] = [];

        let application = this._applicationsManager.getApplicationFrom(destinationPath);
        if (!application) throw new ApplicationConfigurationNotFound(destinationPath);
        context.applicationId = application.id;
        
        const boundedContextPath = path.join(destinationPath, context.name);
        const boundedContextConfigPath = path.join(boundedContextPath, boundedContextFileName);

        
        createdDetails.push(
            this._boilerplates.create(boilerplate, boundedContextPath, context)
        );

        let boundedContextJson = this._filesystem.readJsonSync(boundedContextConfigPath);

        const hasAdornment = context[boundedContextAdornmentDependencyName] !== undefined;

        if (hasAdornment) {
            const adornmentBoilerplateName = context[boundedContextAdornmentDependencyName];
            let adornmentBoilerplate = this.getAdornments(boilerplate.language, boilerplate.name, namespace).find(_ => _.name === adornmentBoilerplateName);
            if (adornmentBoilerplate) {
                createdDetails.push(
                    this._boilerplates.create(adornmentBoilerplate, boundedContextPath, context)
                );
            }
        }

        let interactionLayers: InteractionLayer[] = [];
        let interactionLayerChoices = Object.keys(context).filter(_ => _.startsWith('interaction'));

        if (interactionLayerChoices.length > 0) {
            let interactionLayerNames = interactionLayerChoices.map(prop => context[prop]);
            let interactionLayerBoilerplates = this.getInteractionLayers(boilerplate.language, boilerplate.name, namespace);
            interactionLayerBoilerplates = interactionLayerBoilerplates.filter(boilerplate => interactionLayerNames.includes(boilerplate.name));
            interactionLayerBoilerplates.forEach(boilerplate => {
                let entryPoint = `${boilerplate.target[0].toUpperCase()}${boilerplate.target.slice(1)}`;
                interactionLayers.push(
                    new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint)
                );
                createdDetails.push(
                    this._boilerplates.create(boilerplate, path.join(boundedContextPath, entryPoint), context)
                );
            });
        }
        let boundedContext = new BoundedContext(boundedContextJson.application, boundedContextJson.boundedContext, boundedContextJson.boundedContextName, 
            Resources.fromJson(boundedContextJson.resources), Core.fromJson(boundedContextJson.core), interactionLayers, boundedContextPath);

        this._filesystem.writeJsonSync(boundedContextConfigPath, boundedContext.toJson(), {spaces: 4});

        return createdDetails;
    }

    addInteractionLayer(context: any , boilerplate: ContentBoilerplate, boundedContextFolder: string, entryPoint: string) {
        let boundedContext = this.getNearestBoundedContextConfig(boundedContextFolder);
        if (!boundedContext) throw new Error('Could not discover the bounded context');
        this._boilerplates.create(boilerplate, path.join(getFileDirPath(boundedContext.path), entryPoint), context);
        boundedContext.addInteractionLayer(new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint));
        this._filesystem.writeJsonSync(boundedContext.path, boundedContext.toJson(), {spaces: 4});
    }
    
    addInteractionLayerToBoundedContext(context: any, boilerplate: ContentBoilerplate, boundedContext: BoundedContext, entryPoint: string): BoundedContext {
        this._boilerplates.create(boilerplate, path.join(getFileDirPath(boundedContext.path), entryPoint), context);
        boundedContext.addInteractionLayer(new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint));
        this._filesystem.writeJsonSync(boundedContext.path, boundedContext.toJson(), {spaces: 4});

        return boundedContext;
    }
}