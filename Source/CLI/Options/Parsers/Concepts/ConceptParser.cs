// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Reflection;
using Dolittle.Runtime.Rudimentary;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Dolittle.Runtime.CLI.Options.Parsers.Concepts
{
    /// <summary>
    /// An implementation of <see cref="IValueParser"/> that parses implementations of <see cref="ConceptAs{TValue}"/>.
    /// </summary>
    /// <typeparam name="TConcept">The Concept type.</typeparam>
    /// <typeparam name="TBase">The Concept value type.</typeparam>
    public abstract class ConceptParser<TConcept, TBase> : IValueParser
        where TConcept: ConceptAs<TBase>
    {
        /// <inheritdoc />
        public Type TargetType => typeof(TConcept);

        /// <inheritdoc />
        public object Parse(string argName, string value, CultureInfo culture)
        {
            var constructor = typeof(TConcept).GetConstructor(new[] {typeof(TBase)});
            ThrowIfMissingExpectedConstructor(constructor);

            var parsed = Parse(value, culture);
            return constructor!.Invoke(new object[] {parsed}) as TConcept;
        }

        /// <summary>
        /// Parses the value of the provided argument as the base type.
        /// </summary>
        /// <param name="value">The value of the passed argument.</param>
        /// <param name="culture">The culture that should be used to parse values.</param>
        /// <returns>The parsed <typeparamref name="TBase"/>.</returns>
        protected abstract TBase Parse(string value, CultureInfo culture);

        static void ThrowIfMissingExpectedConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ConceptTypeDoesNotHaveExpectedConstructor(typeof(TConcept), typeof(TBase));
            }
        }
    }
}
