using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions to Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Indicates whether the Type is a simple type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if simple (primitive, string, decimal or concept), otherwise false</returns>
        public static bool IsSimple(this Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>) || type.GetGenericTypeDefinition() == typeof(ConceptAs<>)))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive 
                || type.IsEnum
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal));
            }
    }
}