// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React, { useState } from 'react';
import { useQuery } from '@apollo/client';
import {
    CommandBar,
    Dropdown,
    getTheme,
    IColumn,
    ICommandBarItemProps,
    IDropdownOption,
    Icon,
    ShimmeredDetailsList,
    Stack,
    Text
} from '@fluentui/react';
import { eventHandlersQuery } from './eventHandlersQuery';
import { tenantsQuery } from './tenantsQuery';

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
    const [tenantId, setTenantId] = useState<string>();
    const tenantsQueryResult = useQuery(tenantsQuery);
    const eventHandlersQueryResult = useQuery(eventHandlersQuery, {
        variables: {
            tenantId
        }
    });

    const tenantsOptions = tenantsQueryResult.data?.tenancy?.all.map(_ => {
        return {
            key: _,
            text: _
        } as IDropdownOption;
    });

    const commandBarItems: ICommandBarItemProps[] = [
        {
            key: 'tenant',
            text: 'Tenant',
            onRender: () => {
                const theme = getTheme();

                return (
                    <Stack horizontal style={{ alignItems: 'center' }} tokens={{ childrenGap: 8 }} >
                        <Icon iconName="Database" style={{ color: theme.palette.themePrimary, fontSize: theme.fonts.mediumPlus.fontSize }} />
                        <Text variant="medium">Tenant</Text>
                        <Dropdown
                            style={{ width: 200 }}
                            placeholder="Select tenant"
                            defaultSelectedKey="local"
                            selectedKey={tenantId}
                            onChange={(e, o) => setTenantId(o?.key as string)}
                            options={tenantsOptions} />
                    </Stack>
                );
            }
        }
    ];

    //if (eventHandlersQueryResult.error) return <p>Error :(</p>;

    const result = eventHandlersQueryResult.data?.eventHandlers?.allForTenant || [];

    return (
        <>
            <Stack>
                <CommandBar
                    styles={{
                        root: {
                            alignItems: 'center',
                        },
                    }}
                    items={commandBarItems} />

                <ShimmeredDetailsList columns={columns} items={result} enableShimmer={eventHandlersQueryResult.loading} />
            </Stack>
        </>
    );
};