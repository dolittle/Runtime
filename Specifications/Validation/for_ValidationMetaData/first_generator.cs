// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Validation.MetaData;

namespace Dolittle.Specs.Validation.for_ValidationMetaData
{
    public class first_generator : ICanGenerateValidationMetaData
    {
        public TypeMetaData type_meta_data_to_return;
        public bool generate_for_called;

        public TypeMetaData GenerateFor(Type typeForValidation)
        {
            generate_for_called = true;
            return type_meta_data_to_return;
        }
    }
}
