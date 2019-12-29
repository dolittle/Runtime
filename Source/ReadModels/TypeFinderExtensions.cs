// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Reflection;
using Dolittle.Types;

namespace Dolittle.ReadModels
{
    /// <summary>
    /// Extension methods for <see cref="ITypeFinder"/> for dealing with ReadModels and Queries.
    /// </summary>
    public static class TypeFinderExtensions
    {
        /// <summary>
        /// Get the type of the <see cref="IReadModelOf{T}"/> matching the fullname.  This can be in any loaded assembly and does not require the assmebly qualified name.
        /// </summary>
        /// <param name="typeFinder">instance of <see cref="ITypeFinder"/> being extended.</param>
        /// <param name="fullName">The full name of the type.</param>
        /// <returns>the type if found, <see cref="UnknownReadModelOf" /> if not found or type is not a readmodelof.</returns>
        public static Type GetReadModelOfTypeByName(this ITypeFinder typeFinder, string fullName)
        {
            var readModelOfType = typeFinder.FindTypeByFullName(fullName);

            if (readModelOfType?.HasInterface(typeof(IReadModelOf<>)) != true)
                throw new UnknownReadModelOf(fullName);

            return readModelOfType;
        }
    }
}