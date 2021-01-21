// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents an instance of <see cref="Reason"/>.
    /// </summary>
    public class Cause
    {
        readonly Dictionary<string, string> _arguments = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Cause"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="Reason"/>.</param>
        /// <param name="args">Any arguments for the <see cref="Reason"/>.</param>
        public Cause(Reason reason, object args)
        {
            Reason = reason;
            ExtractArguments(args);

            Title = InterpolateString(reason.Title);
            Description = InterpolateString(reason.Description);
        }

        /// <summary>
        /// Gets the <see cref="Reason"/> the cause if for.
        /// </summary>
        public Reason Reason { get; }

        /// <summary>
        /// Gets the rendered title string.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the rendered description string.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets any arguments passed to the <see cref="Cause"/>.
        /// </summary>
        public IDictionary<string, string> Arguments => _arguments;

        string InterpolateString(string input)
        {
            var result = input;

            var regex = new Regex("{(.*?)}");
            foreach (Match match in regex.Matches(input))
            {
                var propertyName = match.Groups[1].Value;
                if (_arguments.ContainsKey(propertyName))
                {
                    result = result.Replace(match.Groups[0].Value, _arguments[propertyName], StringComparison.InvariantCulture);
                }
            }

            return result;
        }

        void ExtractArguments(object args)
        {
            var type = args.GetType();
            type.GetProperties().ForEach(_ => _arguments[_.Name] = _.GetValue(args).ToString());
            type.GetFields().ForEach(_ => _arguments[_.Name] = _.GetValue(args).ToString());
        }
    }
}
