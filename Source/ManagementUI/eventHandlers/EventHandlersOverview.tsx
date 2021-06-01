// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';
import { useQuery, gql } from '@apollo/client';
import { DetailsList, IColumn, ShimmeredDetailsList } from '@fluentui/react';

const query = gql`
    query {
        eventHandlers {
            allForTenant(tenantId:"709f5739-0f3f-4cac-83bb-7ea3fed6d97d") {
                id
                sourceStream
                position
                failed
            }
        }
    }`;

const columns: IColumn[] = [
    {
        key: 'id',
        fieldName: 'id',
        name: 'Id',
        minWidth: 250
    },
    {
        key: 'sourceStream',
        fieldName: 'sourceStream',
        name: 'Source Stream',
        minWidth: 250
    },
    {
        key: 'position',
        fieldName: 'position',
        name: 'Position',
        minWidth: 100
    },
    {
        key: 'failed',
        fieldName: 'failed',
        name: 'Failed',
        minWidth: 100
    }
];


export const EventHandlersOverview = () => {
    const { loading, error, data } = useQuery(query);

    if (error) return <p>Error :(</p>;

    const result = data?.eventHandlers?.allForTenant || [];

    return (
        <>
            <ShimmeredDetailsList columns={columns} items={result} enableShimmer={loading} />
        </>
    );
};