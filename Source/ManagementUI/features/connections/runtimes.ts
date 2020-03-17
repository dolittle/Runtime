// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { IColumn } from 'office-ui-fabric-react/lib/DetailsList';

export class Runtimes {

    private _allItems: any[];
    private _columns: IColumn[];

    constructor() {
        this._allItems = [];
        for (let i = 0; i < 20; i++) {
            this._allItems.push({
                key: i,
                name: 'Item ' + i,
                value: i
            });
        }

        this._columns = [
            {
                key: 'column1',
                name: 'Name',
                fieldName: 'name',
                minWidth: 100,
                maxWidth: 200,
                isResizable: true
            },
            {
                key: 'column2',
                name: 'Value',
                fieldName: 'value',
                minWidth: 100,
                maxWidth: 200,
                isResizable: true
            }
        ];
    }

    private onItemInvoked = (item: any): void => {
        alert(`Item invoked: `);
    }
}
