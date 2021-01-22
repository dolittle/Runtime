// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents a delegate that is weakly referenced - non obtrusive for the
    /// garbage collector.
    /// </summary>
    public class WeakDelegate
    {
        readonly WeakReference _target;
        readonly MethodInfo _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakDelegate"/> class.
        /// </summary>
        /// <param name="delegate">The actual <see cref="Delegate"/>.</param>
        public WeakDelegate(Delegate @delegate)
        {
            _target = new WeakReference(@delegate.Target);
            _method = @delegate.GetMethodInfo();
        }

        /// <summary>
        /// Gets a value indicating whether or not the reference is alive.
        /// </summary>
        public bool IsAlive => _target.IsAlive || IsStatic;

        /// <summary>
        /// Gets the target instance.
        /// </summary>
        public object Target => _target.Target;

        bool IsStatic => (_method.Attributes & MethodAttributes.Static) == MethodAttributes.Static;

        /// <summary>
        /// Dynamically invoke the delegate on the target if the target is alive.
        /// </summary>
        /// <param name="arguments">Params of arguments to pass to the delegate.</param>
        /// <returns>Return value from the delegate.</returns>
        public object DynamicInvoke(params object[] arguments)
        {
            ThrowIfTargetNotAlive();
            ThrowIfSignatureMismatches(arguments);

            return _method.Invoke(Target, arguments);
        }

        void ThrowIfTargetNotAlive()
        {
            if (!IsAlive) throw new CannotInvokeMethodBecauseTargetIsNotAlive(_method);
        }

        void ThrowIfSignatureMismatches(object[] arguments)
        {
            var parameters = _method.GetParameters();
            if (arguments.Length != parameters.Length) throw new InvalidMethodSignature(_method);

            for (var argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
            {
                if (!parameters[argumentIndex].ParameterType.GetTypeInfo().IsAssignableFrom(arguments[argumentIndex].GetType().GetTypeInfo())) throw new InvalidMethodSignature(_method);
            }
        }
    }
}