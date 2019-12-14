// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Transactions
{
    /// <summary>
    /// Defines a logical transaction.
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback to the state before the transaction started.
        /// </summary>
        void Rollback();
    }
}
