/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using doLittle.Reflection;
using doLittle.Strings;

namespace doLittle.Events.Files
{
    /// <summary>
    /// Represents helper methods for working with property conversion related to Azure Tables and entities
    /// </summary>
    /// <typeparam name="TTarget">Target object <see cref="Type"/></typeparam>
    public class PropertiesFor<TTarget>
    {
        /// <summary>
        /// Get a value from a dictionary of values by giving the target objects property.
        /// From the expression given it will find the expected type and also the name of the property to
        /// look for in the entity from Azure Tables
        /// </summary>
        /// <typeparam name="TProperty"><see cref="Type"/> of property type - inferred</typeparam>
        /// <param name="entity">Dictionary to get from</param>
        /// <param name="property">Property expression</param>
        /// <returns>Value of the property from the dictionary in the form of a string</returns>
        public static string GetValue<TProperty>(IDictionary<string,object> entity, Expression<Func<TTarget, TProperty>> property)
        {
            var propertyInfo = property.GetPropertyInfo();
            var value = string.Empty;

            if( entity.ContainsKey(propertyInfo.Name) )
            {
                value = entity[propertyInfo.Name].ToString();
            } else if( entity.ContainsKey(propertyInfo.Name.ToCamelCase()) ) 
            {
                value = entity[propertyInfo.Name.ToCamelCase()].ToString();
            }

            return value;
        }

        /// <summary>
        /// Get propertyname from a property expression
        /// </summary>
        /// <param name="property">Property expression</param>
        /// <returns>Name of the property</returns>
        public static string GetPropertyName(Expression<Func<TTarget, object>> property)
        {
            var propertyInfo = property.GetPropertyInfo();
            return propertyInfo.Name;
        }
    }
}
