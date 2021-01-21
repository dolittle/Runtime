// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Globalization
{
    /// <summary>
    /// Defines a localizer for entering in and out of a <see cref="LocalizationScope"/>.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Begin a <see cref="LocalizationScope"/>.
        /// </summary>
        /// <returns><see cref="LocalizationScope"/>.</returns>
        LocalizationScope BeginScope();

        /// <summary>
        /// End a <see cref="LocalizationScope"/>.
        /// </summary>
        /// <param name="scope"><see cref="LocalizationScope"/> to end.</param>
        void EndScope(LocalizationScope scope);
    }
}
