// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Represents the result of an operation that either succeeds with a result or fails, with a potential exception.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public class Try<TResult> : Try
{
    protected Try(TResult result) => Result = result;

    protected Try(Exception exception) : base(exception) { }

    /// <summary>
    /// Gets the <typeparamref name="TResult">result</typeparamref>.
    /// </summary>
    public TResult? Result { get; protected set; }
    
    

    /// <summary>
    /// Projects the successfull result if the operation succeeded, or returns the original failure if the operation failed.
    /// </summary>
    /// <param name="selector">A transform function to apply to the result.</param>
    /// <typeparam name="TSelectResult">The type of the projected result.</typeparam>
    /// <returns>A new <see cref="Try{TSelectResult}"/> result that contains the projected result if the operation succeeded.</returns>
    public Try<TSelectResult> Select<TSelectResult>(Func<TResult, TSelectResult> selector)
        => Success
            ? Try<TSelectResult>.Succeeded(selector(Result))
            : Try<TSelectResult>.Failed(Exception);

    /// <summary>
    /// Try to do something that returns a result.
    /// </summary>
    /// <param name="try">What to try do.</param>
    /// <returns>The <see cref="Try{TResult}"/> representing an action that has been tried and resulted in either an exception or a <typeparamref name="TResult"/>.</returns>
    public static Try<TResult> Do(Func<TResult> @try)
    {
        try
        {
            return @try();
        }
        catch (Exception ex)
        {
            return Failed(ex);
        }
    }

    /// <summary>
    /// Try to do something asynchronously that returns a result.
    /// </summary>
    /// <param name="try">What to try do.</param>
    /// <returns>A <see cref="Task<T>" /> that, when resolved, returns the <see cref="Try{TResult}"/> representing an action that has been tried and resulted in either an exception or a <typeparamref name="TResult"/>.</returns>
    public static async Task<Try<TResult>> DoAsync(Func<Task<TResult>> @try)
    {
        try
        {
            return await @try().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Failed(ex);
        }
    }

    /// <summary>
    /// Try to do something asynchronously that returns a result.
    /// </summary>
    /// <param name="try">What to try do.</param>
    /// <returns>A <see cref="Task<T>" /> that, when resolved, returns the <see cref="Try{TResult}"/> representing an action that has been tried and resulted in either an exception or a <typeparamref name="TResult"/>.</returns>
    public static Task<Try<TResult>> DoAsync(Func<TResult> @try) => DoAsync(() => Task.FromResult(@try()));

    /// <summary>
    /// Creates a new <see cref="Try{TResult}"/> result indicating a successful operation.
    /// </summary>
    /// <param name="result">The <see cref="TResult"/> of the operation.</param>
    /// <returns>A new <see cref="Try{TResult}"/> result.</returns>
    public static new Try<TResult> Succeeded(TResult result) => new(result);

    /// <summary>
    /// Creates a new <see cref="Try{TResult}"/> result indicating a failed operation because of an exception.
    /// </summary>
    /// <param name="exception">The <see cref="Exception" /> that caused the operation to fail.</param>
    /// <returns>A new <see cref="Try{TResult}"/> result.</returns>
    public static new Try<TResult> Failed(Exception exception) => new(exception);

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
    public static implicit operator TResult?(Try<TResult> @try) => @try.Result;

    /// <summary>
    /// Implicitly convert <see cref="Try{TResult}" /> to <see cref="Try{TResult}.Exception" />.
    /// </summary>
    /// <param name="try">The <see cref="Try{TResult}" /> to convert.</param>
    /// <return><see cref="Try{TResult}.Exception" />.</return>
    public static implicit operator Exception?(Try<TResult> @try) => @try.Exception;

    /// <summary>
    /// Implicitly convert <typeparamref name="TResult">result</typeparamref> to <see cref="Try{TResult}" />.
    /// </summary>
    /// <param name="result">The <typeparamref name="TResult">result</typeparamref> to convert.</param>
    /// <return><see cref="Try{TResult}" />.</return>
    public static implicit operator Try<TResult>(TResult result) => Succeeded(result);

    /// <summary>
    /// Implicitly convert <see cref="bool" /> to <see cref="Try{TResult}" />.
    /// </summary>
    /// <param name="exception">The <see cref="Exception" /> to convert.</param>
    /// <return><see cref="Try{TResult}" />.</return>
    public static implicit operator Try<TResult>(Exception exception) => Failed(exception);
}
