/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using doLittle.Runtime.Applications;
using doLittle.Commands;

namespace doLittle.Runtime.Commands
{
    /// <summary>
    /// Represents a <see cref="IApplicationResourceType">application resource type</see> for 
    /// <see cref="ICommand">commands</see>
    /// </summary>
    public class CommandApplicationResourceType : IApplicationResourceType
    {
        /// <inheritdoc/>
        public string Identifier => "Command";

        /// <inheritdoc/>
        public Type Type => typeof(ICommand);

        /// <inheritdoc/>
        public ApplicationArea Area => ApplicationAreas.Domain;
    }
}
