// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Guid } from '@dolittle/rudiments';

import { ExecutionContext } from '@dolittle/contracts/Execution/ExecutionContext_pb';

export class ClientFactory {
    create<T extends object>(type: any): T {
        const instance = new type('http://localhost:5000', {} as any) as T;

        for (const property in (instance as any).__proto__) {
            if ((instance as any).__proto__.hasOwnProperty(property)) {
                const member = (instance as any)[property];
                if (member.constructor === Function) {
                    (instance as any)[property] = (request: any, metadata: any) => {
                        if (!metadata) {
                            metadata = {};
                        }

                        const executionContext = new ExecutionContext();

                        const application = Guid.parse('c6912245-892f-4eef-9c3e-7fd410268c40');
                        const microservice = Guid.parse('f39b1f61-d360-4675-b859-53c05c87c0e6');
                        const tenant = Guid.parse('a74fed4a-cd5f-4599-bf98-57dae4062ff0');

                        const correlation = Guid.create();

                        executionContext.setApplication(new Uint8Array(application.bytes));
                        executionContext.setMicroservice(new Uint8Array(microservice.bytes));
                        executionContext.setTenant(new Uint8Array(tenant.bytes));
                        executionContext.setCorrelationid(new Uint8Array(correlation.bytes));

                        metadata['executioncontext-bin'] = Buffer.from(executionContext.serializeBinary()).toString('base64');

                        return member.call(instance, request, metadata);
                    };
                }
            }
        }

        return instance;
    }
}
