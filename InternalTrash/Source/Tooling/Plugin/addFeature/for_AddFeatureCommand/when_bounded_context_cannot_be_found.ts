/*---------------------------------------------------------------------------------------------
*  Copyright (c) Dolittle. All rights reserved.
*  Licensed under the MIT License. See LICENSE in the project root for license information.
*--------------------------------------------------------------------------------------------*/
import { IFileSystem } from "@dolittle/tooling.common.files";
import { ILoggers } from "@dolittle/tooling.common.logging";
import { Exception, ICanOutputMessages, IBusyIndicator } from "@dolittle/tooling.common.utilities";
import { CommandContext, IFailedCommandOutputter } from "@dolittle/tooling.common.commands";
import { IDependencyResolvers } from "@dolittle/tooling.common.dependencies";
import sinon from 'sinon';
import { AddFeatureCommand, IBoundedContextsManager } from "../../internal";
import { expect } from "chai";


describe('when bounded context cannot be found', async () => {

    let command = new AddFeatureCommand(
        {getNearestBoundedContextConfig: sinon.stub().returns(Promise.resolve(null))} as any as IBoundedContextsManager,
        {} as IFileSystem, {} as ILoggers);

    let exception: Exception;
    try {
        await command.onAction(
            {currentWorkingDirectory: 'something'} as any as CommandContext,
            {} as IDependencyResolvers, {} as IFailedCommandOutputter, {} as ICanOutputMessages, {} as IBusyIndicator);
    }
    catch (error) {
        exception = error;
    }
    it('should throw exception', () => expect(exception).to.not.be.undefined);
});