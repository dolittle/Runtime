/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { BoundedContext, boundedContextFileName, Core, InteractionLayer, Resources } from '@dolittle/tooling.common.configurations';
import { PromptDependency, chooseOneUserInputType } from '@dolittle/tooling.common.dependencies';
import { getFileDirPath, IFolders, IFileSystem} from '@dolittle/tooling.common.files';
import { groupBy } from '@dolittle/tooling.common.utilities';
import { ILoggers } from '@dolittle/tooling.common.logging';
import path from 'path';
import { IContentBoilerplates, IContentBoilerplate, CreatedContentBoilerplateDetails } from '@dolittle/tooling.common.boilerplates';
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
     * @param {IFolders} _folders
     * @param {IFileSystem} _fileSystem
     * @param {ILoggers} _logger
     */
    constructor(private _boilerplates: IContentBoilerplates, private _applicationsManager: IApplicationsManager,
        private _folders: IFolders, private _filesystem: IFileSystem, private _logger: ILoggers) {}
    
    get boilerplates() {
        return this._boilerplates.byType(boundedContextBoilerplateType) as IContentBoilerplate[];
    }

    async getNearestBoundedContextConfig(startPath: string) {
        let regex =  new RegExp('\\b'+boundedContextFileName+'\\b');
        let boundedContextConfigPaths = await this._folders.getNearestFilesSearchingUpwards(startPath, regex);

        if (boundedContextConfigPaths.length === 0 || boundedContextConfigPaths[0] === '') return null;

        const boundedContextConfigPath = boundedContextConfigPaths[0];
        this._logger.info(`Found bounded context configuration at path '${boundedContextConfigPath}'`);

        let boundedContextObj = await this._filesystem.readJson(boundedContextConfigPath);
        let boundedContext = BoundedContext.fromJson(boundedContextObj, boundedContextConfigPath);
        
        return boundedContext;
    }

    hasBoundedContext(folder: string) {
        const filePath = path.join(folder, boundedContextFileName);
        return this._filesystem.exists(filePath);
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

    async create(context: any, boilerplate: IContentBoilerplate, destinationPath: string, namespace?: string) {
        let createdDetails: CreatedContentBoilerplateDetails[] = [];

        let application = await this._applicationsManager.getApplicationFrom(destinationPath);
        if (!application) throw new ApplicationConfigurationNotFound(destinationPath);
        context.applicationId = application.id;
        
        const boundedContextPath = path.join(destinationPath, context.name);

        createdDetails.push(
            await this._boilerplates.create(boilerplate, boundedContextPath, context)
        );
        
        await this.createAdornments(context, boilerplate, namespace, boundedContextPath, createdDetails)
        
        let interactionLayers = await this.createInteractionLayers(context, boilerplate, namespace, boundedContextPath, createdDetails);
        
        if (interactionLayers.length > 0) this.addInteractionLayersToBoundedContextConfigurationFile(boundedContextPath, interactionLayers);

        return createdDetails;
    }

    async addInteractionLayer(context: any , boilerplate: IContentBoilerplate, boundedContextFolder: string, entryPoint: string) {
        let boundedContext = await this.getNearestBoundedContextConfig(boundedContextFolder);
        if (!boundedContext) throw new Error('Could not discover the bounded context');
        this._boilerplates.create(boilerplate, path.join(getFileDirPath(boundedContext.path), entryPoint), context);
        boundedContext.addInteractionLayer(new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint));
        await this._filesystem.writeJsonSync(boundedContext.path, boundedContext.toJson(), {spaces: 4});
    }
    
   async addInteractionLayerToBoundedContext(context: any, boilerplate: IContentBoilerplate, boundedContext: BoundedContext, entryPoint: string) {
        this._boilerplates.create(boilerplate, path.join(getFileDirPath(boundedContext.path), entryPoint), context);
        boundedContext.addInteractionLayer(new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint));
        await this._filesystem.writeJson(boundedContext.path, boundedContext.toJson(), {spaces: 4});

        return boundedContext;
    }

    private async createAdornments(context: any, boilerplate: IContentBoilerplate, namespace: string | undefined, boundedContextPath: string, createdDetails: CreatedContentBoilerplateDetails[]) {
        const hasAdornment = context[boundedContextAdornmentDependencyName] !== undefined;

        if (hasAdornment) {
            const adornmentBoilerplateName = context[boundedContextAdornmentDependencyName];
            let adornmentBoilerplate = this.getAdornments(boilerplate.language, boilerplate.name, namespace).find(_ => _.name === adornmentBoilerplateName);
            if (adornmentBoilerplate) {
                createdDetails.push(
                    await this._boilerplates.create(adornmentBoilerplate, boundedContextPath, context)
                );
            }
        }

    }

    private async createInteractionLayers(context: any, boilerplate: IContentBoilerplate, namespace: string | undefined, boundedContextPath: string, createdDetails: CreatedContentBoilerplateDetails[]) {

        let interactionLayers: InteractionLayer[] = [];
        let interactionLayerChoices = Object.keys(context).filter(_ => _.startsWith('interaction'));

        if (interactionLayerChoices.length > 0) {
            let interactionLayerNames = interactionLayerChoices.map(prop => context[prop]);
            let interactionLayerBoilerplates = this.getInteractionLayers(boilerplate.language, boilerplate.name, namespace);
            interactionLayerBoilerplates = interactionLayerBoilerplates.filter(boilerplate => interactionLayerNames.includes(boilerplate.name));
            
            await Promise.all(interactionLayerBoilerplates.map(async boilerplate => {
                let entryPoint = `${boilerplate.target[0].toUpperCase()}${boilerplate.target.slice(1)}`;
                interactionLayers.push(
                    new InteractionLayer(boilerplate.type, boilerplate.language, boilerplate.framework, entryPoint)
                );
                createdDetails.push(
                    await this._boilerplates.create(boilerplate, path.join(boundedContextPath, entryPoint), context)
                );
            }));
        }
        return interactionLayers;
    }

    private async addInteractionLayersToBoundedContextConfigurationFile(boundedContextPath: string, interactionLayers: InteractionLayer[]) {

        const boundedContextConfigPath = path.join(boundedContextPath, boundedContextFileName);
        let boundedContextJson = await this._filesystem.readJson(boundedContextConfigPath);
        let boundedContext = new BoundedContext(boundedContextJson.application, boundedContextJson.boundedContext, boundedContextJson.boundedContextName, 
            Resources.fromJson(boundedContextJson.resources), Core.fromJson(boundedContextJson.core), interactionLayers, boundedContextPath);

        return this._filesystem.writeJson(boundedContextConfigPath, boundedContext.toJson(), {spaces: 4});
    }
}
