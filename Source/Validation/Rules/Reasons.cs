// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Contains common <see cref="Reason">reasons</see> for broken validation rules.
    /// </summary>
    public static class Reasons
    {
        /// <summary>
        /// When a value is equal and it is not not allowed to be equal, this is the reason given.
        /// </summary>
        public static Reason ValueIsEqual = Reason.Create("CEFA9147-5F13-4C82-B609-C64582EC33AB", "Value {LeftHand} is equal {RightHand}");

        /// <summary>
        /// When a value is less than the specified greater than value, this is the reason given.
        /// </summary>
        public static Reason ValueIsLessThan = Reason.Create("8CFB5B51-55E6-41A6-A01A-33F83E141CF2", "Value {LeftHand} is less than {RightHand}");

        /// <summary>
        /// When a value was greater than the specified less than value, this is the reason given.
        /// </summary>
        public static Reason ValueIsGreaterThan = Reason.Create("6C489DB3-DE0A-45BA-A547-5A6E3AD3F303", "Value {LeftHand} is greater than {RightHand}");

        /// <summary>
        /// When something is longer than it should, this is the reason given.
        /// </summary>
        public static Reason LengthIsTooLong = Reason.Create("D9675214-A6A4-439F-8D8E-AF0A48BD1BF0", "Length {Length} is too long");

        /// <summary>
        /// When something is longer than it should, this is the reason given.
        /// </summary>
        public static Reason LengthIsTooShort = Reason.Create("E0F8D478-A353-4926-893E-DD367E2F2ACF", "Length {Length} is too short");
    }
}
