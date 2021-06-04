// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React, { useState } from 'react';
import { useLazyQuery, useQuery } from '@apollo/client';
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
        key: 'scope',
        fieldName: 'scope',
        name: 'Scope',
        minWidth: 250
    },
    {
        key: 'filterPosition',
        fieldName: 'filterPosition',
        name: 'Filter Position',
        minWidth: 100
    },
    {
        key: 'eventProcessorPosition',
        fieldName: 'eventProcessorPosition',
        name: 'Event Processor Position',
        minWidth: 100
    },
    {
        key: 'tailEventLogSequenceNumber',
        fieldName: 'tailEventLogSequenceNumber',
        name: 'Tail Event Log Sequence Number',
        minWidth: 100
    }


    
];


export const EventHandlersOverview = () => {
    const [tenantId, setTenantId] = useState<string>();
    const tenantsQueryResult = useQuery(tenantsQuery);
    const [getEventHandlers, eventHandlersQueryResult] = useLazyQuery(eventHandlersQuery, {
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
                            onChange={(e, o) => {
                                setTenantId(o?.key as string);
                                getEventHandlers();
                            }}
                            options={tenantsOptions} />
                    </Stack>
                );
            }
        },
        {
            key: 'refresh',
            text: 'Refresh',
            iconProps: { iconName: 'Refresh' },
            onClick: () => getEventHandlers()
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