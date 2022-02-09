// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable DL0008

namespace Dolittle.Runtime.Collections;

/// <summary>
/// Represents an implementation of <see cref="IDictionary{TKey, TValue}"/> that filters out
/// null keys and/or values.
/// </summary>
/// <typeparam name="TKey">Key type.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
public class NullFreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    readonly IDictionary<TKey, TValue> _dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullFreeDictionary{TKey, TValue}"/> class.
    /// </summary>
    public NullFreeDictionary()
    {
        _dict = new Dictionary<TKey, TValue>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullFreeDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="otherDictionary">The <see cref="IDictionary{TKey, TValue}"/> to populate this with.</param>
    public NullFreeDictionary(IDictionary<TKey, TValue> otherDictionary)
    {
        _dict = new Dictionary<TKey, TValue>();
        foreach (var keyValue in otherDictionary)
            Add(keyValue);
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dict.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => _dict.Values;

    /// <inheritdoc/>
    public int Count => _dict.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get { return _dict[key]; }
        set { _dict[key] = value; }
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (value != null)
        {
            _dict[key] = value;
        }
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _dict.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dict.Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _dict.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _dict.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        return _dict.Remove(key);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _dict.Remove(item);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dict.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dict.GetEnumerator();
    }
}