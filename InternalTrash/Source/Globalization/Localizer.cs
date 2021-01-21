// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace Dolittle.Globalization
{
    /// <summary>
    /// Represents a <see cref="ILocalizer"/>.
    /// </summary>
    public class Localizer : ILocalizer
    {
        /// <inheritdoc/>
        public LocalizationScope BeginScope()
        {
            return LocalizationScope.FromCurrentThread();
        }

        /// <inheritdoc/>
        public void EndScope(LocalizationScope scope)
        {
            CultureInfo.CurrentCulture = scope.Culture;
            CultureInfo.CurrentUICulture = scope.UICulture;
        }
    }
}