// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Types;

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents an implementation of <see cref="IValidationMetaData"/>.
    /// </summary>
    public class ValidationMetaData : IValidationMetaData
    {
        readonly IInstancesOf<ICanGenerateValidationMetaData> _generators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationMetaData"/> class.
        /// </summary>
        /// <param name="generators">Instances of <see cref="ICanGenerateValidationMetaData"/>.</param>
        public ValidationMetaData(IInstancesOf<ICanGenerateValidationMetaData> generators)
        {
            _generators = generators;
        }

        /// <inheritdoc/>
        public TypeMetaData GetMetaDataFor(Type typeForValidation)
        {
            var typeMetaData = new TypeMetaData();

            foreach (var generator in _generators)
            {
                var metaData = generator.GenerateFor(typeForValidation);

                foreach (var property in metaData.Properties.Keys)
                {
                    foreach (var ruleSet in metaData.Properties[property].Keys)
                    {
                        typeMetaData[property][ruleSet] = metaData.Properties[property][ruleSet];
                    }
                }
            }

            return typeMetaData;
        }
    }
}
