// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary;

public static class TryExtensions
{
    #region Synchronous input and output

    static Try<TNextResult> Then<TResult, TNextResult>(Try<TResult> result, Func<Try<TResult>, Try<TNextResult>> onSuccess, Func<Try<TResult>, Try<TNextResult>> onFailure)
    {
        return result.Success ? onSuccess(result) : onFailure(result);
    }

    /// <summary>
    /// Adds a continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Try<TNextResult> Then<TResult, TNextResult>(this Try<TResult> result, Func<TResult, Try<TNextResult>> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    return callback(_.Result);
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Try<TNextResult>.Failed(_.Exception));

    /// <summary>
    /// Adds a continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Try<TNextResult> Then<TResult, TNextResult>(this Try<TResult> result, Func<TResult, TNextResult> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    return Try<TNextResult>.Succeeded(callback(_.Result));
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Try<TNextResult>.Failed(_.Exception));

    /// <summary>
    /// Adds a callback to a <see cref="Try{TResult}"/> that will be called if the operation is successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>The original operation result.</returns>
    public static Try<TResult> Then<TResult>(this Try<TResult> result, Action<TResult> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    callback(_.Result);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            },
            _ => _);

    /// <summary>
    /// Adds a catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Try<TResult> Catch<TResult>(this Try<TResult> result, Func<Exception, Try<TResult>> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    return callback(_.Exception);
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds a catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Try<TResult> Catch<TResult>(this Try<TResult> result, Func<Exception, TResult> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    return Try<TResult>.Succeeded(callback(_.Exception));
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds a catch callback to a <see cref="Try{TResult}"/> that will be called if the operation failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>The original operation result.</returns>
    public static Try<TResult> Catch<TResult>(this Try<TResult> result, Action<Exception> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    callback(_.Exception);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    #endregion
    
    #region Synchronous input and asynchronous output

    static Task<Try<TNextResult>> Then<TResult, TNextResult>(Try<TResult> result, Func<Try<TResult>, Task<Try<TNextResult>>> onSuccess, Func<Try<TResult>, Task<Try<TNextResult>>> onFailure)
    {
        return result.Success ? onSuccess(result) : onFailure(result);
    }

    /// <summary>
    /// Adds an asynchronous continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Try<TResult> result, Func<TResult, Task<Try<TNextResult>>> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    return await callback(_.Result).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Task.FromResult(Try<TNextResult>.Failed(_.Exception)));

    /// <summary>
    /// Adds an asynchronous continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Try<TResult> result, Func<TResult, Task<TNextResult>> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    return Try<TNextResult>.Succeeded(await callback(_.Result).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Task.FromResult(Try<TNextResult>.Failed(_.Exception)));

    /// <summary>
    /// Adds an asynchronous callback to a <see cref="Try{TResult}"/> that will be called if the operation is successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Then<TResult>(this Try<TResult> result, Func<TResult, Task> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    await callback(_.Result).ConfigureAwait(false);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            },
            Task.FromResult);

    /// <summary>
    /// Adds an asynchronous catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Try<TResult> result, Func<Exception, Task<Try<TResult>>> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    return await callback(_.Exception).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds an asynchronous catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Try<TResult> result, Func<Exception, Task<TResult>> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    return Try<TResult>.Succeeded(await callback(_.Exception).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds an asynchronous catch callback to a <see cref="Try{TResult}"/> that will be called if the operation failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Try<TResult> result, Func<Exception, Task> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    await callback(_.Exception).ConfigureAwait(false);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });
    
    #endregion

    #region Asynchronous input and synchronous output

    static async Task<Try<TNextResult>> Then<TResult, TNextResult>(Task<Try<TResult>> task, Func<Try<TResult>, Try<TNextResult>> onSuccess, Func<Try<TResult>, Try<TNextResult>> onFailure)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return result.Success ? onSuccess(result) : onFailure(result);
        }
        catch (Exception exception)
        {
            return onFailure(Try<TResult>.Failed(exception));
        }
    }

    /// <summary>
    /// Adds a continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Task<Try<TResult>> result, Func<TResult, Try<TNextResult>> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    return callback(_.Result);
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Try<TNextResult>.Failed(_.Exception));

    /// <summary>
    /// Adds a continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Task<Try<TResult>> result, Func<TResult, TNextResult> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    return Try<TNextResult>.Succeeded(callback(_.Result));
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Try<TNextResult>.Failed(_.Exception));

    /// <summary>
    /// Adds a callback to a <see cref="Try{TResult}"/> that will be called if the operation is successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Then<TResult>(this Task<Try<TResult>> result, Action<TResult> callback)
        => Then(
            result,
            _ =>
            {
                try
                {
                    callback(_.Result);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            },
            _ => Try<TResult>.Failed(_.Exception));

    /// <summary>
    /// Adds a catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result, Func<Exception, Try<TResult>> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    return callback(_.Exception);
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds a catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result, Func<Exception, TResult> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    return Try<TResult>.Succeeded(callback(_.Exception));
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds a catch callback to a <see cref="Try{TResult}"/> that will be called if the operation failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result, Action<Exception> callback)
        => Then(
            result,
            _ => _,
            _ =>
            {
                try
                {
                    callback(_.Exception);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });
    
    #endregion
    
    #region Asynchronous input and output

    static async Task<Try<TNextResult>> Then<TResult, TNextResult>(Task<Try<TResult>> task, Func<Try<TResult>, Task<Try<TNextResult>>> onSuccess, Func<Try<TResult>, Task<Try<TNextResult>>> onFailure)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return result.Success
                ? await onSuccess(result).ConfigureAwait(false)
                : await onFailure(result).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return await onFailure(Try<TResult>.Failed(exception)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Adds an asynchronous continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Task<Try<TResult>> result, Func<TResult, Task<Try<TNextResult>>> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    return await callback(_.Result).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Task.FromResult(Try<TNextResult>.Failed(_.Exception)));

    /// <summary>
    /// Adds an asynchronous continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <typeparam name="TNextResult">The type of the result after the continuation.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TNextResult>> Then<TResult, TNextResult>(this Task<Try<TResult>> result,
        Func<TResult, Task<TNextResult>> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    return Try<TNextResult>.Succeeded(await callback(_.Result).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return Try<TNextResult>.Failed(exception);
                }
            },
            _ => Task.FromResult(Try<TNextResult>.Failed(_.Exception)));

    /// <summary>
    /// Adds an asynchronous callback to a <see cref="Try{TResult}"/> that will be called if the operation is successful.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation succeeded.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Then<TResult>(this Task<Try<TResult>> result, Func<TResult, Task> callback)
        => Then(
            result,
            async _ =>
            {
                try
                {
                    await callback(_.Result).ConfigureAwait(false);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            },
            Task.FromResult);

    /// <summary>
    /// Adds an asynchronous catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result,
        Func<Exception, Task<Try<TResult>>> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    return await callback(_.Exception).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds an asynchronous catch continuation to a <see cref="Try{TResult}"/> that will be called to provide a new result based on the previous if it failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a new <see cref="Try{TResult}"/> containing the result of the callback.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result,
        Func<Exception, Task<TResult>> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    return Try<TResult>.Succeeded(await callback(_.Exception).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });

    /// <summary>
    /// Adds an asynchronous catch callback to a <see cref="Try{TResult}"/> that will be called if the operation failed.
    /// </summary>
    /// <param name="result">The result of the previous operation.</param>
    /// <param name="callback">The asynchronous callback to call when the previous operation failed.</param>
    /// <typeparam name="TResult">The type of the result to continue on.</typeparam>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the original operation result.</returns>
    public static Task<Try<TResult>> Catch<TResult>(this Task<Try<TResult>> result, Func<Exception, Task> callback)
        => Then(
            result,
            Task.FromResult,
            async _ =>
            {
                try
                {
                    await callback(_.Exception).ConfigureAwait(false);
                    return _;
                }
                catch (Exception exception)
                {
                    return Try<TResult>.Failed(exception);
                }
            });
    
    #endregion
}
