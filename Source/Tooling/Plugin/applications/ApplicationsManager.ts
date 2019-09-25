/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Application, applicationFilename } from '@dolittle/tooling.common.configurations';
import {IFileSystem} from '@dolittle/tooling.common.files';
import { ILoggers } from '@dolittle/tooling.common.logging';
import path from 'path';
import { ContentBoilerplate, CreatedContentBoilerplateDetails, IContentBoilerplates, IBoilerplatesLoader } from '@dolittle/tooling.common.boilerplates';
import {IApplicationsManager} from '../internal';

export const applicationBoilerplateType = 'application';

/**
 * Represents an implementation of {IApplicationsManager} for managing 
 *
 * @export
 * @class ArtifactsManager
 */
export class ApplicationsManager implements IApplicationsManager {
    /**
     * Instantiates an instance of {ApplicationsManager}.
     * @param {IContentBoilerplates} _boilerplates
     * @param {IFileSystem} _fileSystem
     * @param {ILoggers} _logger
     */
    constructor(private _boilerplates: IContentBoilerplates, private _boilerplatesLoader: IBoilerplatesLoader, private _fileSystem: IFileSystem, private _logger: ILoggers) {}

    async getBoilerplates() {
        if (this._boilerplatesLoader.needsReload) await this._boilerplatesLoader.load();
        return this._boilerplates.byType(applicationBoilerplateType);
    }

    async getApplicationFrom(folder: string) {
        if (! this.hasApplication(folder)) return null;
        const filePath = path.join(folder, applicationFilename);
        let json = await this._fileSystem.readJson(filePath);
        return Application.fromJson(json, filePath);
    }

    hasApplication(folder: string) {
        const filePath = path.join(folder, applicationFilename);
        return this._fileSystem.exists(filePath);
    }

    async getBoilerplatesByLanguage(language: string, namespace?: string) {
        if (this._boilerplatesLoader.needsReload) await this._boilerplatesLoader.load();
        return this._boilerplates.byLanguageAndType(language, applicationBoilerplateType, namespace);
    }

    async create(context: any, destinationPath: string, boilerplate: ContentBoilerplate): Promise<CreatedContentBoilerplateDetails[]> {
        this._logger.info(`Creating an application with language '${boilerplate.language}' at destination ${destinationPath}`);
        
        let createdDetails: CreatedContentBoilerplateDetails[] = [];
        let createdApplicationDetails = await this._boilerplates.create(boilerplate, destinationPath, context);
        
        createdDetails.push(createdApplicationDetails);
        
        return createdDetails;
    }
}