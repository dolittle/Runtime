// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { PrimaryButton } from '@fluentui/react';
import React from 'react';
import { useQuery, gql } from '@apollo/client';
import { Layout } from './layouts/Layout';
import { BrowserRouter as Router } from 'react-router-dom';

import './App.scss';

const query = gql`
    query {
    eventHandlers {
        allForTenant(tenantId:"709f5739-0f3f-4cac-83bb-7ea3fed6d97d") {
            id
        }
    }
    }`;

export const App = () => {
    const { loading, error, data } = useQuery(query);

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error :(</p>;

    return (
        <>
            <Router>
                <Layout />
                <div className="content">
                    <PrimaryButton>Hello there</PrimaryButton>

                </div>
            </Router>
        </>
    )
};