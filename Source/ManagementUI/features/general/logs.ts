// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { autoinject } from 'aurelia-dependency-injection';

import { LogClient } from '@dolittle/runtime.contracts.web/Logging.Management/Log_grpc_web_pb';
import { LogMessage, LogMessages } from '@dolittle/runtime.contracts.web/Logging.Management/Log_pb';

import { IColumn, ISelection, Selection, SelectionMode } from 'office-ui-fabric-react/lib/DetailsList';

import { DialogType, IDialogProps } from 'office-ui-fabric-react/lib/Dialog';

import { ClientFactory } from '../ClientFactory';

import { Empty } from 'google-protobuf/google/protobuf/empty_pb';
import * as grpc from 'grpc';

@autoinject
export class Logs {
    log: any[] = [];

    commandBarItems: any[] = [
        {
            key: 'download',
            name: 'Download',
            iconProps: {
                iconName: 'Download'
            },
            onClick: () => this.actionButtonClick()
        },
        {
            key: 'upload',
            name: 'Upload',
            iconProps: {
                iconName: 'Upload'
            },
            onClick: () => console.log('Upload')
        }
    ];

    commandBarFarItems: any[] = [
        {
            key: 'sort',
            name: 'Sort',
            iconProps: {
                iconName: 'SortLines'
            },
            onClick: () => console.log('Sort')
        },
        {
            key: 'tile',
            name: 'Grid view',
            iconProps: {
                iconName: 'Tiles'
            },
            iconOnly: true,
            onClick: () => console.log('Tiles')
        },
        {
            key: 'info',
            name: 'Info',
            iconProps: {
                iconName: 'Info'
            },
            iconOnly: true,
            onClick: () => console.log('Info')
        }
    ];

    hidden: boolean = true;

    dialogprops: IDialogProps =
        {
            dialogContentProps: {
                type: DialogType.normal,
                title: 'Download log',
                subText: 'This will download the log. Logs might contain sensitive data.'
            },
            modalProps: {
                titleAriaId: 'myLabelId',
                subtitleAriaId: 'mySubTextId',
                isBlocking: false,
                containerClassName: 'ms-dialogMainOverride'
            },
            onDismiss: () => {
                if (this.dialogprops && this.dialogprops.dialogContentProps && this.dialogprops.dialogContentProps.title) {
                    this.dialogprops.dialogContentProps.title += '!';
                }
                this.hidden = true;
            },
            hidden: this.hidden
        };

    columns: IColumn[] = [
        {
            key: 'timestamp',
            name: 'Timestamp',
            fieldName: 'timestamp',
            minWidth: 50,
            maxWidth: 100,
            isResizable: true
        },
        {
            key: 'message',
            name: 'Message',
            fieldName: 'message',
            minWidth: 100,
            maxWidth: 200,
            isResizable: true
        }

    ];
    selectedHead: any;
    selectionMode: SelectionMode = SelectionMode.none;
    selection: ISelection;

    private _client: LogClient;
    private _listenCall: grpc.ClientReadableStream<LogMessage> = null as any;

    constructor(private _clientFactory: ClientFactory) {
        this._client = _clientFactory.create(LogClient);
        this.selection = new Selection();
    }

    attached() {
        this._listenCall = this._client.listen(new Empty());
        this._listenCall.on('data', (response: LogMessages) => {
            response.getMessagesList().forEach((message: LogMessage) => {
                (window as any)._log = this.log;
                this.log.unshift({
                    timestamp: message.getTimestamp()?.toDate().toLocaleTimeString(),
                    message: message.getMessage()
                });

                this.log = this.log.slice(0, 20);
            });
        });

        this._listenCall.on('status', (status: any) => {
            console.log(status);
        });

        this._listenCall.on('end', () => {
            console.log('End');
        });

        this._listenCall.on('error', (error: Error) => {
            console.log(error);
        });
    }

    detached() {
        this._listenCall?.cancel();
    }

    actionButtonClick(this: any) {
        this.hidden = !this.hidden;
    }
}
