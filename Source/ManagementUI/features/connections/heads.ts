// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { autoinject } from 'aurelia-dependency-injection';

import { HeadsClient } from '@dolittle/runtime.contracts.web/Heads.Management/Heads_grpc_web_pb';
import { ConnectedHead, ConnectedHeadsRequest, ConnectedHeadsResponse } from '@dolittle/runtime.contracts.web/Heads.Management/Heads_pb';

import { ClientFactory } from '../ClientFactory';

import { IColumn, ISelection, Selection, SelectionMode } from 'office-ui-fabric-react/lib/DetailsList';

import { PanelType } from 'office-ui-fabric-react/lib/Panel';

import { Guid } from '@dolittle/rudiments';

import * as grpc from "grpc";

@autoinject
export class Heads {
    allHeads: any[] = [];
    columns: IColumn[] = [
        {
            key: 'identifier',
            name: 'Identifier',
            fieldName: 'id',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        },
        {
            key: 'host',
            name: 'Host',
            fieldName: 'host',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        },
        {
            key: 'runtime',
            name: 'Runtime',
            fieldName: 'runtime',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        },
        {
            key: 'version',
            name: 'Version',
            fieldName: 'version',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        },
        {
            key: 'connected',
            name: 'Connected',
            fieldName: 'connectionTime',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        }
    ];

    headSelected: boolean = false;
    panelHeaderText: string = '';
    panelType: PanelType = PanelType.smallFixedFar;
    selectedHead: any;
    selectionMode: SelectionMode = SelectionMode.none;
    selection: ISelection;

    private _client: HeadsClient;
    private _getConnectedHeadsCall: grpc.ClientReadableStream<ConnectedHeadsResponse> = null as any;

    constructor(private _clientFactory: ClientFactory) {
        this._client = _clientFactory.create(HeadsClient);
        this.selection = new Selection();
    }

    detached() {
        this._getConnectedHeadsCall?.cancel();
    }

    attached() {
        const request = new ConnectedHeadsRequest();
        this._getConnectedHeadsCall = this._client.getConnectedHeads(request);
        this._getConnectedHeadsCall.on('data', (response: ConnectedHeadsResponse) => {
            const heads = response.getHeadsList();
            this.allHeads = heads.map((connectedHead: ConnectedHead) => {
                const pbHead = connectedHead.getHead();
                if (pbHead) {
                    return {
                        id: new Guid(pbHead.getHeadid_asU8()).toString(),
                        host: pbHead.getHost(),
                        runtime: pbHead.getRuntime(),
                        version: pbHead.getVersion(),
                        connectionTime: pbHead.getConnectiontime()?.toDate().toString()
                    };
                }
                return null;
            });
        });

        /*
        call.on('status', (status: Status) => {
        });

        call.on('error', (error: Error) => {
        });

        call.on('end', () => {
        });*/
    }

    itemSelected(item: any) {
        this.selectedHead = item;
        this.headSelected = true;
    }

    panelDismissed = () => {
        this.headSelected = false;
        this.selectedHead = undefined;
    }
}
