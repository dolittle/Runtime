// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

#pragma warning disable DL0008

namespace Dolittle.Runtime.Collections;

/// <summary>
/// Represent an immutable <see cref="IReadOnlyList{T}" /> of objects that does not allow nulls as elements.
/// </summary>
/// <typeparam name="T">Tye type of elements in the list.</typeparam>
public class ReadOnlyNullFreeList<T> : IReadOnlyList<T>
{
    readonly NullFreeList<T> _elements;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyNullFreeList{T}"/> class.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public ReadOnlyNullFreeList(IEnumerable<T> collection)
    {
        _elements = new NullFreeList<T>(collection);
    }

    /// <inheritdoc/>
    public int Count => _elements.Count;

    /// <inheritdoc/>
    public T this[int index] => _elements[index];

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return _elements.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _elements.GetEnumerator();
    }
}