// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Dolittle.Rules;
using Dolittle.Validation;

namespace Dolittle.Runtime.Commands
{
    /// <summary>
    /// Represents the result after handling a <see cref="CommandRequest"/>.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets the name of command that this result is related to.
        /// </summary>
        public CommandRequest Command { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}">collection</see> of <see cref="BrokenRule">broken rules</see>.
        /// </summary>
        public IEnumerable<BrokenRuleResult> BrokenRules { get; set; } = Array.Empty<BrokenRuleResult>();

        /// <summary>
        /// Gets or sets the ValidationResults generated during handling of a command.
        /// </summary>
        public IEnumerable<ValidationResult> ValidationResults { get; set; } = Array.Empty<ValidationResult>();

        /// <summary>
        /// Gets or sets the error messages that are related to full command during validation.
        /// </summary>
        public IEnumerable<string> CommandValidationMessages { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the messages that are related to broken security rules.
        /// </summary>
        public IEnumerable<string> SecurityMessages { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets any validation errors (for properties or for the full command) as a simple string enumerbale.
        /// To relate property validation errors to the relevant property, use the <see cref="ValidationResult">ValidationResults</see> property.
        /// </summary>
        public IEnumerable<string> AllValidationMessages
        {
            get { return CommandValidationMessages.Union(ValidationResults.Select(vr => vr.ErrorMessage)); }
        }

        /// <summary>
        /// Gets or sets the exception, if any, that occured during a handle.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the exception message, if any.
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets a value indicating whether the command is successful or not.
        /// </summary>
        /// <remarks>
        /// If there are invalid validationresult or command validattion messages, this is false.
        /// If an exception occured, this is false.
        /// Otherwise, its true.
        /// </remarks>
        public bool Success => Exception == null && PassedSecurity && !Invalid && !HasBrokenRules;

        /// <summary>
        /// Gets a value indicating whether or not the command is valid.
        /// </summary>
        /// <remarks>
        /// If there are any validationresults or command validation messages this returns false, true if not.
        /// </remarks>
        public bool Invalid => (ValidationResults?.Any() == true) || (CommandValidationMessages?.Any() == true);

        /// <summary>
        /// Gets a value indicating whether or not command passed security.
        /// </summary>
        public bool PassedSecurity => SecurityMessages?.Any() == false;

        /// <summary>
        /// Gets a value indicating whether or not there are any broken rules.
        /// </summary>
        public bool HasBrokenRules => BrokenRules.Any();

        /// <summary>
        /// Create a <see cref="CommandResult"/> for a given <see cref="CommandRequest"/> instance.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/> to create from.</param>
        /// <returns>A <see cref="CommandResult"/> with <see cref="CommandRequest"/> details populated.</returns>
        public static CommandResult ForCommand(CommandRequest command)
        {
            return new CommandResult
            {
                Command = command
            };
        }

        /// <summary>
        /// Merges another CommandResult instance into the current instance.
        /// </summary>
        /// <param name="commandResultToMerge">The source <see cref="CommandResult"/> to merge into current instance.</param>
        public void MergeWith(CommandResult commandResultToMerge)
        {
            if (Exception == null)
                Exception = commandResultToMerge.Exception;

            MergeValidationResults(commandResultToMerge);
            MergeCommandErrorMessages(commandResultToMerge);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Success : {0}", Success);
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ", Invalid : {0}", Invalid);

            if (Exception != null)
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ", Exception : {0}", Exception.Message);

            if (ExceptionMessage != null)
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ", ExceptionMesssage : {0}", ExceptionMessage);

            return stringBuilder.ToString();
        }

        void MergeValidationResults(CommandResult commandResultToMerge)
        {
            if (commandResultToMerge.ValidationResults == null)
                return;

            if (ValidationResults == null)
            {
                ValidationResults = commandResultToMerge.ValidationResults;
                return;
            }

            var validationResults = ValidationResults.ToList();
            validationResults.AddRange(commandResultToMerge.ValidationResults);
            ValidationResults = validationResults.ToArray();
        }

        void MergeCommandErrorMessages(CommandResult commandResultToMerge)
        {
            if (commandResultToMerge.CommandValidationMessages == null)
                return;

            if (CommandValidationMessages == null)
            {
                CommandValidationMessages = commandResultToMerge.CommandValidationMessages;
                return;
            }

            var commandErrorMessages = CommandValidationMessages.ToList();
            commandErrorMessages.AddRange(commandResultToMerge.CommandValidationMessages);
            CommandValidationMessages = commandErrorMessages.ToArray();
        }
    }
}