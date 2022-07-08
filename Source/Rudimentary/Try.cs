// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Represents the result of an operation that either succeeds or fails, with a potential exception.
/// </summary>
public class Try
{
    protected Try() => Success = true;

    protected Try(Exception exception)
    {
        Success = false;
        Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the <see cref="Exception" /> that caused the operation to fail.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Appends the result if the operation succeeded, or returns the original failure if the operation failed.
    /// </summary>
    /// <param name="result">The result to append.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>A new <see cref="Try{TSelectResult}"/> result that contains the appended result if the operation succeeded.</returns>
    public Try<TResult> With<TResult>(TResult result)
        => Success
            ? Try<TResult>.Succeeded(result)
            : Try<TResult>.Failed(Exception);

    /// <summary>
    /// Try to do something.
    /// </summary>
    /// <param name="try">What to try do.</param>
    /// <returns>The <see cref="Try"/> representing an action that has been tried.</returns>
    public static Try Do(Action @try)
    {
        try
        {
            @try();
            return Succeeded();
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
    /// <returns>A <see cref="Task<T>" /> that, when resolved, returns the <see cref="Try"/> representing an action that has been tried and resulted in either an exception or nothing.</returns>
    public static async Task<Try> DoAsync(Func<Task> @try)
    {
        try
        {
            await @try().ConfigureAwait(false);
            return Succeeded();
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
    /// <returns>A <see cref="Task<T>" /> that, when resolved, returns the <see cref="Try"/> representing an action that has been tried and resulted in either an exception or nothing.</returns>
    public static Task<Try> DoAsync(Action @try) => DoAsync(() => Task.Run(@try));

    /// <summary>
    /// Creates a new <see cref="Try"/> result indicating a successful operation.
    /// </summary>
    /// <returns>A new <see cref="Try"/> result.</returns>
    public static Try Succeeded() => new();

    /// <summary>
    /// Creates a new <see cref="Try"/> result indicating a failed operation because of an exception.
    /// </summary>
    /// <param name="exception">The <see cref="Exception" /> that caused the operation to fail.</param>
    /// <returns>A new <see cref="Try"/> result.</returns>
    public static Try Failed(Exception exception) => new(exception);

    /// <summary>
    /// Implicitly convert <see cref="Try" /> to <see cref="Success" />.
    /// </summary>
    /// <param name="try">The <see cref="Try" /> to convert.</param>
    /// <return><see cref="Success" />.</return>
    public static implicit operator bool(Try @try) => @try.Success;

    /// <summary>
    /// Implicitly convert <see cref="Try" /> to <see cref="Exception" />.
    /// </summary>
    /// <param name="try">The <see cref="Try" /> to convert.</param>
    /// <return><see cref="Exception" />.</return>
    /// public static implicit operator Exception(Try @try) => @try.Exception;

    /// <summary>
    /// Implicitly convert <see cref="bool" /> to <see cref="Try" />.
    /// </summary>
    /// <param name="exception">The <see cref="System.Exception" /> to convert.</param>
    /// <return><see cref="Try" />.</return>
    public static implicit operator Try(Exception exception) => Failed(exception);
}