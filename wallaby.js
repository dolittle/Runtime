/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
const build = require('@dolittle/typescript.build');

module.exports = build.wallaby(undefined, w => {
    require('reflect-metadata')
});
