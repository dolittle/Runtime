// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary
{
    /// <summary>
    /// Represents the result of an operation that either succeeds with a result or fails, with a potential exception.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public class Partial<TResult> : Try<TResult>
    {
        protected Partial(TResult result) : base(result)
        {
        }

        protected Partial(TResult result, Exception exception) : base(exception)
        {
            Result = result;
            IsPartialResult = true;
        }

        protected Partial() : base() { }

        protected Partial(Exception exception) : base(exception) { }

        /// <summary>
        /// Gets a value indicating whether this is a partial result. 
        /// </summary>
        public bool IsPartialResult { get; }

        /// <summary>
        /// Creates a new <see cref="Partial{TResult}"/> result indicating a successful operation.
        /// </summary>
        /// <param name="result">The <see cref="TResult"/> of the operation.</param>
        /// <returns>A new <see cref="Partial{TResult}"/> result.</returns>
        public static new Partial<TResult> Succeeded(TResult result) => new(result);

        /// <summary>
        /// Creates a new <see cref="Partial{TResult}"/> result indicating a successful operation.
        /// </summary>
        /// <param name="result">The <see cref="TResult"/> of the operation.</param>
        /// <returns>A new <see cref="Partial{TResult}"/> result.</returns>
        public static Partial<TResult> PartialSuccess(TResult result, Exception exception = default) => new(result, exception);

        /// <summary>
        /// Creates a new <see cref="Partial{TResult}"/> result indicating a failed operation.
        /// </summary>
        /// <returns>A new <see cref="Partial{TResult}"/> result.</returns>
        public static new Partial<TResult> Failed() => new();

        /// <summary>
        /// Creates a new <see cref="Partial{TResult}"/> result indicating a failed operation because of an exception.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> that caused the operation to fail.</param>
        /// <returns>A new <see cref="Partial{TResult}"/> result.</returns>
        public static new Partial<TResult> Failed(Exception exception) => new(exception);

        /// <summary>
        /// Implicitly convert <typeparamref name="TResult">result</typeparamref> to <see cref="Try{TResult}" />.
        /// </summary>
        /// <param name="result">The <typeparamref name="TResult">resulet</typeparamref> to convert.</param>
        /// <return><see cref="Try{TResult}" />.</return>
        public static implicit operator Partial<TResult>(TResult result) => Succeeded(result);

        /// <summary>
        /// Implicitly convert <see cref="bool" /> to <see cref="Try{TResult}" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to convert.</param>
        /// /// /// /// <return><see cref="Try{TResult}" />.</return>
        public static implicit operator Partial<TResult>(Exception exception) => Failed(exception);
    }
}