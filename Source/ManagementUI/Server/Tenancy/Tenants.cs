// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Tenancy;
using Microsoft.AspNetCore.Mvc;

namespace Server.Tenancy
{
    /// <summary>
    /// Represents the controller for working with tenants.
    /// </summary>
    [Route("/api/Tenants")]
    public class Tenants : Controller
    {
        readonly ITenants _tenants;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenants"/> class.
        /// </summary>
        /// <param name="tenants">Underlying <see cref="ITenants"/>.</param>
        public Tenants(ITenants tenants)
        {
            _tenants = tenants;
        }

        /// <summary>
        /// Gets all the available tenants in the system.
        /// </summary>
        /// <returns>All tenants as JSON.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Json(_tenants.All);
        }
    }
}