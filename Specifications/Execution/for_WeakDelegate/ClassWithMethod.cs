// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Execution.for_WeakDelegate
{
    public class ClassWithMethod
    {
        public const int ReturnValue = 42;

        public int SomeMethod(string stringParameter, double intParameter)
        {
            return ReturnValue;
        }

        public int SomeOtherMethod(IInterface input)
        {
            return ReturnValue;
        }
    }
}
