// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Concepts
{
    /// <summary>
    /// Expresses a Concept as a another type, usually a primitive such as Guid, Int or String.
    /// </summary>
    /// <typeparam name="T">Type of the concept.</typeparam>
    public class ConceptAs<T> : IEquatable<ConceptAs<T>>, IComparable<ConceptAs<T>>, IComparable
    {
        /// <summary>
        /// Gets the underlying primitive type of this concept.
        /// </summary>
        public static Type UnderlyingType => typeof(T);

        /// <summary>
        /// Gets or sets the underlying primitive value of this concept.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Implicitly convert from <see cref="ConceptAs{T}"/> to type of the <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="value">The converted value.</param>
        public static implicit operator T(ConceptAs<T> value) => value == null ? default : value.Value;

        /// <summary>
        /// Equality operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is equal to the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator ==(ConceptAs<T> left, ConceptAs<T> right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is equal to the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator !=(ConceptAs<T> left, ConceptAs<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Larger than comparison operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is larger than the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator >(ConceptAs<T> left, ConceptAs<T> right)
        {
            return left.CompareTo(right) == 1;
        }

        /// <summary>
        /// Smaller than comparison operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is smaller than the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator <(ConceptAs<T> left, ConceptAs<T> right)
        {
            return left.CompareTo(right) == -1;
        }

        /// <summary>
        /// Larger than or equal comparison operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is larger or equal than the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator >=(ConceptAs<T> left, ConceptAs<T> right)
        {
            return left.CompareTo(right) > -1;
        }

        /// <summary>
        /// Smaller or equal than comparison operator for comparing two <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="left">Left <see cref="ConceptAs{T}"/>.</param>
        /// <param name="right">Right <see cref="ConceptAs{T}"/>.</param>
        /// <returns>true if the left <see cref="ConceptAs{T}"/> is smaller or equal than the right <see cref="ConceptAs{T}"/>; otherwise, false.</returns>
        public static bool operator <=(ConceptAs<T> left, ConceptAs<T> right)
        {
            return left.CompareTo(right) < 1;
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public virtual bool Equals(ConceptAs<T> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            var t = GetType();
            var otherType = other.GetType();

            return t == otherType && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Check if <see cref="ConceptAs{T}"/> is empty.
        /// </summary>
        /// <returns>true if empty, false if not.</returns>
        /// <remarks>
        /// If the value within the concept is null, it is concidered to be empty.
        /// If the value within is of type string and its length is 0, it is concidered to be empty.
        /// If the value is equal to the default value of the type of concept, it is concidered to be empty.
        /// </remarks>
        public bool IsEmpty()
        {
            if (Value == null)
                return true;

            if (Value is string value)
                return value?.Length == 0;

            return Value.Equals(default(T));
        }

        /// <summary>
        /// Determines how two <see cref="ConceptAs{T}"/> instances compares to each other.
        /// </summary>
        /// <param name="other">The <see cref="ConceptAs{T}"/> to compare with the current <see cref="ConceptAs{T}"/>.</param>
        /// <returns>Comparison result.</returns>
        public virtual int CompareTo(ConceptAs<T> other)
        {
            return other == null ? 1 : Comparer<T>.Default.Compare(Value, other.Value);
        }

        /// <summary>
        /// Determines how the <see cref="ConceptAs{T}"/> instance compares to an <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ConceptAs{T}"/> to compare with the current <see cref="ConceptAs{T}"/>.</param>
        /// <returns>Comparison result.</returns>
        public virtual int CompareTo(object obj)
        {
            var other = obj as ConceptAs<T>;
            return CompareTo(other);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value == null ? default(T).ToString() : Value.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ConceptAs<T> typed)
            {
                return Equals(typed);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCodeHelper.Generate(typeof(T), Value);
        }
    }
}