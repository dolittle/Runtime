/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Applications
{
    /// <summary>
    /// Defines a module within a <see cref="IBoundedContext"/>
    /// </summary>
    public interface IModule : IApplicationLocation<ModuleName>, IBelongToAnApplicationLocationTypeOf<IBoundedContext>, ICanHaveApplicationLocationsOfType<IFeature>
    {
        /// <summary>
        /// Add a feature to the <see cref="Module"/>
        /// </summary>
        /// <param name="feature"></param>
        void AddFeature(IFeature feature);
    }
}
