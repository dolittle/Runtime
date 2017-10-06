/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Applications
{
    /// <summary>
    /// Defines a <see cref="Feature">feature</see> inside a <see cref="Feature">feature</see>
    /// </summary>
    public interface ISubFeature : IFeature, IBelongToAnApplicationLocationTypeOf<IFeature>
    {
    }
}
