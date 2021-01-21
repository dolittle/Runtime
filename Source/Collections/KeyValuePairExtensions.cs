// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Collections.Generic
{
    /// <summary>
    /// Extends <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    public static class KeyValuePairExtensions
    {
        /// <summary>
        /// Adds support for KeyValuePair deconstruction.
        /// </summary>
        /// <param name="keyValuePair">KeyValuePair to deconstruct.</param>
        /// <param name="key">Output key.</param>
        /// <param name="value">Output value.</param>
        /// <typeparam name="TKey">Type of key - will be automatically inferred.</typeparam>
        /// <typeparam name="TValue">Type of value - will be automatically inferred.</typeparam>
        /// <remarks>
        /// Read more about deconstruct https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct.
        /// </remarks>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
        {
            key = keyValuePair.Key;
            value = keyValuePair.Value;
        }
    }
}