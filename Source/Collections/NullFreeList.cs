// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable DL0008

namespace Dolittle.Runtime.Collections
{
    /// <summary>
    /// Represent an <see cref="IList{T}" /> of objects that does not allow nulls as elements.
    /// </summary>
    /// <typeparam name="T">Tye type of elements in the list.</typeparam>
    public class NullFreeList<T> : IList<T>
    {
        readonly IList<T> _elements = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NullFreeList{T}"/> class.
        /// </summary>
        public NullFreeList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullFreeList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public NullFreeList(IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                if (element == null) throw new ArgumentNullException(nameof(collection), $"{nameof(collection)} can not contain null elements");
                _elements.Add(element);
            }
        }

        /// <inheritdoc/>
        public int Count => _elements.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _elements.IsReadOnly;

        /// <inheritdoc/>
        public T this[int index]
        {
            get => _elements[index];
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _elements[index] = value;
            }
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _elements.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            if (item == null) return false;
            return _elements.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            if (item == null) return -1;
            return _elements.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _elements.Insert(index, item);
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            if (item == null) return false;
            return _elements.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

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
}