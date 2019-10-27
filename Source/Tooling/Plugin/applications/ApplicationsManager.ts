/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Application, applicationFilename } from '@dolittle/tooling.common.configurations';
import {IFileSystem} from '@dolittle/tooling.common.files';
import { ILoggers } from '@dolittle/tooling.common.logging';
import path from 'path';
import { ContentBoilerplate, CreatedContentBoilerplateDetails, IContentBoilerplates } from '@dolittle/tooling.common.boilerplates';
import {IApplicationsManager} from '../index';

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
    constructor(private _boilerplates: IContentBoilerplates, private _fileSystem: IFileSystem, private _logger: ILoggers) {}

    get boilerplates() {
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

    boilerplatesByLanguage(language: string, namespace?: string) {
        let boilerplates = this.boilerplates;
        return boilerplates.filter( _ => {
            if (namespace && _.namespace) return _.namespace === namespace && _.language === language;
            return _.language && language; 
        });
    }

    async create(context: any, destinationPath: string, boilerplate: ContentBoilerplate): Promise<CreatedContentBoilerplateDetails[]> {
        this._logger.info(`Creating an application with language '${boilerplate.language}' at destination ${destinationPath}`);
        
        let createdDetails: CreatedContentBoilerplateDetails[] = [];
        let createdApplicationDetails = await this._boilerplates.create(boilerplate, destinationPath, context);
        
        createdDetails.push(createdApplicationDetails);
        
        return createdDetails;
    }
}