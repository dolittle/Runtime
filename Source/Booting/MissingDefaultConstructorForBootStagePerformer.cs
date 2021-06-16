// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="ICanPerformPartOfBootStage"/> is missing a default constructor.
    /// </summary>
    public class MissingDefaultConstructorForBootStagePerformer : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingDefaultConstructorForBootStagePerformer"/> class.
        /// </summary>
        /// <param name="type">Type of <see cref="ICanPerformPartOfBootStage"/> that is misssing the default constructor.</param>
        public MissingDefaultConstructorForBootStagePerformer(Type type)
            : base($"Boot stage performer of type '{type.AssemblyQualifiedName}' is missing a default constructor")
        {
        }
    }
}
