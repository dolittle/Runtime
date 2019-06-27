/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import {ICanProvideNamespaces, INamespace } from "@dolittle/tooling.common.commands";

/**
 * Represents an implementation of {ICanProvideNamespaces} for providing the namespace for the runtime plugin
 *
 * @export
 * @class NamespaceProvider
 * @implements {ICanProvideNamespaces}
 */
export class NamespaceProvider implements ICanProvideNamespaces {

    constructor(private _namespaces: INamespace[]) {}

    provide(): INamespace[] {
        return this._namespaces;
    }

}