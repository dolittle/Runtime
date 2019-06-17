/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { Command } from "@dolittle/tooling.common.commands";
import { ITemplate, ITemplatesBoilerplate, chooseTemplate, ITemplatesBoilerplates } from "@dolittle/tooling.common.boilerplates";
import { ICanOutputMessages, NullMessageOutputter, IBusyIndicator, NullBusyIndicator, determineDestination } from "@dolittle/tooling.common.utilities";
import { IDependencyResolvers, IDependency } from "@dolittle/tooling.common.dependencies";
import { MissingBoundedContext, IBoundedContextsManager } from "../index";
import { Folders } from "@dolittle/tooling.common.files";

export class AddCommand extends Command {

    private _templates: ITemplate[];
    private _templatesBoilerplate: ITemplatesBoilerplate;
    private _templatesBoilerplates: ITemplatesBoilerplates;
    private _dependencyResolvers: IDependencyResolvers;
    private _boundedContextsManager: IBoundedContextsManager;
    private _folders: Folders;
    private _dolittleConfig: any;

    /**
     * Creates an instance of {AddCommand}.
     * @param {string} artifactType
     * @param {ITemplate[]} artifactTemplates
     * @memberof Installed
     */
    constructor(templatesBoilerplate: ITemplatesBoilerplate, templateType: string, templates: ITemplate[], templatesBoilerplates: ITemplatesBoilerplates, dependencyResolvers: IDependencyResolvers, 
                boundedContextsManager: IBoundedContextsManager, folders: Folders, dolittleConfig: any) {
        if (!templates || templates.length === 0) throw new Error('No templates given to add command');
        super(templateType, templates[0].description, undefined, [templatesBoilerplate.nameDependency, ...templatesBoilerplate.dependencies]);

        this._templates = templates;
        this._templatesBoilerplate = templatesBoilerplate;
        this._dependencyResolvers = dependencyResolvers;
        this._boundedContextsManager = boundedContextsManager;
        this._dolittleConfig = dolittleConfig;
        this._folders = folders;
        this._templatesBoilerplates = templatesBoilerplates;
    }

    async action(cwd: string, coreLanguage: string, commandArguments?: string[], options?: Map<string, any>, namespace?: string, 
                outputter: ICanOutputMessages = new NullMessageOutputter(), busyIndicator: IBusyIndicator = new NullBusyIndicator()) {
       
        let templatesWithLanguage = this._templates.filter(_ => this._templatesBoilerplate.namespace === namespace && this._templatesBoilerplate.language === coreLanguage);

        if (!templatesWithLanguage.length || templatesWithLanguage.length < 1) {
            outputter.warn(`There are no artifact templates of type '${this.name}' with language '${coreLanguage}'${namespace? ' under namespace \'' + namespace + '\'' : ''}`);
            return;
        }
        
        let template: ITemplate | null = templatesWithLanguage[0];

        if (templatesWithLanguage.length > 1) {
            do {
                template = await chooseTemplate(templatesWithLanguage, this._dependencyResolvers); 
            } while(!template)
        }

        let dependencies = template.getAllDependencies(this._templatesBoilerplate);
        
        let boundedContext = this._boundedContextsManager.getNearestBoundedContextConfig(cwd);
        if (!boundedContext) throw new MissingBoundedContext(cwd);

        let nameDependency = this._templatesBoilerplate.nameDependency;
        
        let context = await this._dependencyResolvers.resolve({}, [nameDependency], undefined, undefined, commandArguments);
        let destinationAndName = determineDestination(template.area, this._templatesBoilerplate.language, context[nameDependency.name], cwd, boundedContext.path, this._dolittleConfig);
        this._folders.makeFolderIfNotExists(destinationAndName.destination);
        context = await this._dependencyResolvers.resolve(context, dependencies, destinationAndName.destination, coreLanguage, commandArguments, options);
        context[nameDependency.name] = destinationAndName.name;
        
        this._templatesBoilerplates.create(context, template, this._templatesBoilerplate, destinationAndName.destination);
    }
    getAllDependencies(currentWorkingDirectory: string, coreLanguage: string, commandArguments?: string[], commandOptions?: Map<string, any>, namespace?: string): IDependency[] {

        let templatesWithLanguage = this._templates.filter(_ => this._templatesBoilerplate.namespace === namespace && this._templatesBoilerplate.language === coreLanguage);
        let dependencies = templatesWithLanguage[0]? 
            templatesWithLanguage[0].getAllDependencies(this._templatesBoilerplate)
            : [];
        return this.dependencies.concat(dependencies);
    }
}