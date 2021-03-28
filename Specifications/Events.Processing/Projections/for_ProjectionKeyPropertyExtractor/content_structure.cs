// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeyPropertyExtractor
{
    public class content_structure
    {
        public string case_sensitive { get; set; }
        public string CASE_SENSITIVE { get; set; }
        public string CaseSensitive { get; set; }
        public string caseSensitive { get; set; }
        public string a_string_property { get; set; }
        public int a_number_property { get; set; }
        public Guid a_guid_property { get; set; }
        public inner_structure inner_structure { get; set; }

        public static content_structure create()
            => new()
            {
                case_sensitive = nameof(case_sensitive),
                CASE_SENSITIVE = nameof(CASE_SENSITIVE),
                CaseSensitive = nameof(CaseSensitive),
                caseSensitive = nameof(caseSensitive),
                a_string_property = nameof(a_string_property),
                a_number_property = 41,
                a_guid_property = Guid.Empty,
                inner_structure = new inner_structure()
            };

    }

    public class inner_structure
    {
        public string inner_property { get; set; } = nameof(inner_property);
    }
}