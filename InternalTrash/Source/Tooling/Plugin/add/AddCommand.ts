/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { Command, CommandContext, IFailedCommandOutputter } from "@dolittle/tooling.common.commands";
import { ITemplate, ITemplatesBoilerplate, chooseTemplate, ITemplatesBoilerplates } from "@dolittle/tooling.common.boilerplates";
import { ICanOutputMessages, IBusyIndicator, determineDestination } from "@dolittle/tooling.common.utilities";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import { MissingBoundedContext, IBoundedContextsManager } from "../internal";
import { IFolders } from "@dolittle/tooling.common.files";

export class AddCommand extends Command {

    private _templates: ITemplate[];
    private _templatesBoilerplate: ITemplatesBoilerplate;
    private _templatesBoilerplates: ITemplatesBoilerplates;
    private _boundedContextsManager: IBoundedContextsManager;
    private _folders: IFolders;
    private _dolittleConfig: any;

    constructor(templatesBoilerplate: ITemplatesBoilerplate, templateType: string, templates: ITemplate[], templatesBoilerplates: ITemplatesBoilerplates, 
                boundedContextsManager: IBoundedContextsManager, folders: IFolders, dolittleConfig: any) {
        if (!templates || templates.length === 0) throw new Error('No templates given to add command');
        super(templateType, templates[0].description, true,  undefined, [templatesBoilerplate.nameDependency, ...templatesBoilerplate.dependencies.dependencies]);

        this._templates = templates;
        this._templatesBoilerplate = templatesBoilerplate;
        this._boundedContextsManager = boundedContextsManager;
        this._dolittleConfig = dolittleConfig;
        this._folders = folders;
        this._templatesBoilerplates = templatesBoilerplates;
    }

    async onAction(commandContext: CommandContext, dependencyResolvers: IDependencyResolvers, failedCommandOutputter: IFailedCommandOutputter, outputter: ICanOutputMessages, busyIndicator: IBusyIndicator) {
       
        let template = await this.chooseATemplate(dependencyResolvers, outputter, commandContext.coreLanguage, commandContext.namespace);
        if (template === null) return;

        try {
            let boundedContext = await this._boundedContextsManager.getNearestBoundedContextConfig(commandContext.currentWorkingDirectory);
            if (!boundedContext) throw new MissingBoundedContext(commandContext.currentWorkingDirectory);

            let nameDependency = this._templatesBoilerplate.nameDependency;
            
            let context = await dependencyResolvers.resolve({}, [nameDependency], [], commandContext.currentWorkingDirectory, commandContext.coreLanguage);
            let destinationAndName = determineDestination(template.area, this._templatesBoilerplate.language, context[nameDependency.name], commandContext.currentWorkingDirectory, boundedContext.path, this._dolittleConfig);
            
            context[nameDependency.name] = destinationAndName.name;

            await this._folders.makeFolderIfNotExists(destinationAndName.destination);

            let dependencies = template.getAllDependencies(this._templatesBoilerplate);
            context = await dependencyResolvers.resolve(context, dependencies, [], destinationAndName.destination, commandContext.coreLanguage);
            
            await this._templatesBoilerplates.create(context, template, this._templatesBoilerplate, destinationAndName.destination);
        }
        catch (error) {
            failedCommandOutputter.output(this, commandContext, error, template.getAllDependencies(this._templatesBoilerplate));
        }
        
    }

    private async chooseATemplate(dependencyResolvers: IDependencyResolvers, outputter: ICanOutputMessages, coreLanguage: string, namespace?: string) {
        let templatesWithLanguage = this._templates.filter(_ => {
            if (this._templatesBoilerplate.namespace) return this._templatesBoilerplate.namespace === namespace && this._templatesBoilerplate.language === coreLanguage;
            return this._templatesBoilerplate.language === coreLanguage;
        });
        
        if (!templatesWithLanguage.length || templatesWithLanguage.length === 0) {
            outputter.warn(`There are no artifact templates of type '${this.name}' with language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''}`);
            return null;
        }
        
        let template: ITemplate | null = templatesWithLanguage[0];

        if (templatesWithLanguage.length > 1) {
            do {
                template = await chooseTemplate(templatesWithLanguage, dependencyResolvers); 
            } while(!template)
        }
        return template;
    } 
}