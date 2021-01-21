// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Dolittle.Immutability;

namespace Dolittle.Dynamic
{
    /// <summary>
    /// Represents an ExpandoObject that can only have values assigned to during creation.
    /// Similar to <see cref="ExpandoObject"/>, members are dynamic and can be added on the fly.
    /// </summary>
    public class WriteOnceExpandoObject : DynamicObject, IDictionary<string, object>, IAmImmutable
    {
        readonly Dictionary<string, object> _actualDictionary = new Dictionary<string, object>();
        readonly bool _construction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnceExpandoObject"/> class.
        /// </summary>
        /// <param name="populate">Action that gets called during creation for populate the object.</param>
        public WriteOnceExpandoObject(Action<dynamic> populate)
        {
            _construction = true;
            populate(this);
            _construction = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteOnceExpandoObject"/> class.
        /// </summary>
        /// <param name="values">A dictionary of values used to populate the object.</param>
        public WriteOnceExpandoObject(IDictionary<string, object> values)
        {
            _construction = true;
            foreach (var v in values)
            {
                Add(v.Key, v.Value);
            }

            _construction = false;
        }

        /// <inheritdoc/>
        public int Count => _actualDictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public ICollection<string> Keys => _actualDictionary.Keys;

        /// <inheritdoc/>
        public ICollection<object> Values => _actualDictionary.Values;

        /// <inheritdoc/>
        public object this[string key]
        {
            get => _actualDictionary[key];
            set
            {
                ThrowIfNotUnderConstruction();
                _actualDictionary[key] = value;
            }
        }

        /// <summary>
        /// Returns an Dictionary{string,object} representation.
        /// </summary>
        /// <returns>A <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public IDictionary<string, object> AsDictionary() => new Dictionary<string, object>(_actualDictionary);

        /// <inheritdoc/>
        public void Add(KeyValuePair<string, object> item)
        {
            ThrowIfNotUnderConstruction();
            _actualDictionary.Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ThrowIfNotUnderConstruction();
            _actualDictionary.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return _actualDictionary.ContainsKey(item.Key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, object> item)
        {
            ThrowIfNotUnderConstruction();
            return _actualDictionary.Remove(item.Key);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _actualDictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        /// <inheritdoc/>
        public void Add(string key, object value)
        {
            ThrowIfNotUnderConstruction();
            _actualDictionary.Add(key, value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return _actualDictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(string key)
        {
            ThrowIfNotUnderConstruction();
            return _actualDictionary.Remove(key);
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _actualDictionary.TryGetValue(binder.Name, out result);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, out object value)
        {
            return _actualDictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _actualDictionary.GetEnumerator();
        }

        void ThrowIfNotUnderConstruction()
        {
            if (!_construction)
                throw new CannotWriteToAnImmutable();
        }
    }
}
