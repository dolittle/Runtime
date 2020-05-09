// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Async
{
    /// <summary>
    /// Represents something.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public class Try<TResult>
        where TResult : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Try{TResult}"/> class.
        /// </summary>
        /// <param name="success">Whether the try-get operation was successful.</param>
        /// <param name="result">The result.</param>
        public Try(bool success, TResult result)
        {
            Success = success;
            Result = result;
        }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the <typeparamref name="TResult">result</typeparamref>.
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Success" />.
        /// </summary>
        /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
        /// <return><see cref="Try{TResult}.Success" />.</return>
        public static implicit operator bool(Try<TResult> @try) => @try.Success;

        /// <summary>
        /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Result" />.
        /// </summary>
        /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
        /// <return><see cref="Try{TResult}.Result" />.</return>
        public static implicit operator TResult(Try<TResult> @try) => @try.Result;

        /// <summary>
        /// Implicitly convert <typeparamref name="TResult">result</typeparamref> to <see cref="Try{TResult}" />.
        /// </summary>
        /// <param name="result">The <typeparamref name="TResult">result</typeparamref> to convert.</param>
        /// <return><see cref="Try{TResult}" />.</return>
        public static implicit operator Try<TResult>(TResult result) => new Try<TResult>(true, result);

        /// <summary>
        /// Implicitly convert <see cref="bool" /> to <see cref="Try{TResult}" />.
        /// </summary>
        /// <param name="success">The <see cref="bool" /> to convert.</param>
        /// <return><see cref="Try{TResult}" />.</return>
        public static implicit operator Try<TResult>(bool success) => new Try<TResult>(success, null);

        /// <summary>
        /// Implicitly convert <see cref="bool" /> and <typeparamref name="TResult" /> tuple to <see cref="Try{TResult}" />.
        /// </summary>
        /// <param name="try">The <see cref="bool" /> and <typeparamref name="TResult" /> tuple to convert.</param>
        /// <return><see cref="Try{TResult}" />.</return>
        public static implicit operator Try<TResult>((bool success, TResult result) @try) => new Try<TResult>(@try.success, @try.result);
    }
}