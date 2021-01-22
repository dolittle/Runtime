// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Serialization.Json.Specs.for_Serializer
{
    public class Complex : Value<Complex>
    {
        public Complex(Guid concept, Immutable immutable, int primitive, Dictionary<string, object> content)
        {
            Concept = concept;
            Immutable = immutable;
            Primitive = primitive;
            Content = content;
        }

        public Guid Concept { get; }

        public Immutable Immutable { get; }

        public int Primitive { get; }

        public IDictionary<string, object> Content { get; }

        public override bool Equals(Complex other)
        {
            if (other == null)
                return false;

            if (Concept != other.Concept || Immutable != other.Immutable || Primitive != other.Primitive)
                return false;

            if ((Content != null && other.Content == null) || (Content == null && other.Content != null))
                return false;

            if (Content == null && other.Content == null)
                return true;

            return Content.Count == other.Content.Count
                        && Content.Keys.All(key => other.Content.ContainsKey(key)
                                                                && Content[key].Equals(Convert.ChangeType(other.Content[key], Content[key].GetType())));
        }
    }
}