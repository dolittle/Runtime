namespace Dolittle.Runtime.Events.Store
{
    //TODO: should not be in Events.Store ... should move to DotNet.Fundamentals

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Ensures that parameters or properties on parameters are not null, throwing the appropriate ArgumentNullException if they are. 
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the argument is not null, throwing an ArgumentNullException if it is.
        /// </summary>
        /// <param name="parameterName">The name of the argument to be used in the exception</param>
        /// <param name="argument">The instance of the argument</param>
        /// <typeparam name="T">The type of the argument to be checked</typeparam>
        [DebuggerStepThrough]
        public static void IsNotNull<T>(string parameterName, T argument) where T : class
        {
            if (argument == null) 
                ThrowException(parameterName);
        }

        /// <summary>
        /// Ensures that the nullable argument is not null.
        /// </summary>
        /// <param name="parameterName">The name of the argument to be used in the exception</param>
        /// <param name="argument">The instance of the argument</param>
        /// <typeparam name="T">The type of the arugment to be checked</typeparam>
        [DebuggerStepThrough]
        public static void NullableIsNotNull<T>(string parameterName, T argument)
        {
            if (ReferenceEquals(argument, null))
                ThrowException(parameterName);
        }

        /// <summary>
        /// Ensures that the specified property on the argument is not null
        /// </summary>
        /// <param name="parameterName">Name of the parameter to be used in the exception</param>
        /// <param name="propertyPath">The path to the parameter to be used in the exception</param>
        /// <param name="argumentProperty">The instance of the argument</param>
        /// <typeparam name="T">The type of the argument</typeparam>
        [DebuggerStepThrough]
        public static void ArgumentPropertyIsNotNull<T>(string parameterName, string propertyPath, T argumentProperty) where T : class
        {
            if (argumentProperty == null)
            {
                ThrowException(parameterName, propertyPath);
            }
        }

        /// <summary>
        /// Ensures that the specified nullable property on the argument is not null
        /// </summary>
        /// <param name="parameterName">Name of the parameter to be used in the exception</param>
        /// <param name="propertyPath">The path to the parameter to be used in the exception</param>
        /// <param name="argumentProperty">The instance of the argument</param>
        /// <typeparam name="T">The type of the argument</typeparam>
        [DebuggerStepThrough]
        public static void NullableArgumentPropertyIsNotNull<T>(string parameterName, string propertyPath, T argumentProperty)
        {
            if (ReferenceEquals(argumentProperty, null))
            {
                ThrowException(parameterName,propertyPath);
            }
        }

        static void ThrowException(string parameterName, string propertyPath = null)
        {
            var path = propertyPath == null ? string.Empty : $".{propertyPath}";
            throw new ArgumentNullException(parameterName, $"{parameterName}{path} cannot be null");
        }
    }
}