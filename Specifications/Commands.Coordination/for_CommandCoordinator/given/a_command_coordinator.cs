using System.Dynamic;
using doLittle.Applications;
using doLittle.Globalization;
using doLittle.Logging;
using doLittle.Runtime.Commands.Security;
using doLittle.Runtime.Commands.Validation;
using doLittle.Runtime.Transactions;
using doLittle.Security;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Commands.Coordination.Specs.for_CommandCoordinator.given
{
    public class a_command_coordinator
    {
        protected static CommandCoordinator coordinator;
        protected static Mock<ICommandHandlerManager> command_handler_manager_mock;
        protected static Mock<ICommandContextManager> command_context_manager_mock;
        protected static Mock<ICommandSecurityManager> command_security_manager_mock;
        protected static Mock<ICommandValidators> command_validators_mock;
        protected static Mock<ICommandContext> command_context_mock;
        protected static Mock<ILocalizer> localizer_mock;
        protected static Mock<ILogger> logger;
        protected static CommandRequest command;

        Establish context = ()=>
        {
            command = new CommandRequest(TransactionCorrelationId.NotSet, Mock.Of<IApplicationArtifactIdentifier>(), new ExpandoObject());
            command_handler_manager_mock = new Mock<ICommandHandlerManager>();
            command_context_manager_mock = new Mock<ICommandContextManager>();
            command_validators_mock = new Mock<ICommandValidators>();

            command_context_mock = new Mock<ICommandContext>();
            command_context_manager_mock.Setup(c => c.EstablishForCommand(Moq.It.IsAny<CommandRequest>())).
            Returns(command_context_mock.Object);
            command_security_manager_mock = new Mock<ICommandSecurityManager>();
            command_security_manager_mock.Setup(
                    s => s.Authorize(Moq.It.IsAny<CommandRequest>()))
                .Returns(new AuthorizationResult());

            logger = new Mock<ILogger>();
            localizer_mock = new Mock<ILocalizer>();
            localizer_mock.Setup(l => l.BeginScope()).Returns(LocalizationScope.FromCurrentThread);

            coordinator = new CommandCoordinator(
                command_handler_manager_mock.Object,
                command_context_manager_mock.Object,
                command_security_manager_mock.Object,
                command_validators_mock.Object,
                localizer_mock.Object,
                logger.Object
            );
        };
    }
}