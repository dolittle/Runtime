using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Execution;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Binds up the bindings related to the running application. The<see cref="Application"/>, the <see cref="BoundedContext"/> and the <see cref="Environment"/>
    /// </summary>
    public class ApplicationBindings : ICanProvideBindings
    {
        internal static Application Application = Application.NotSet;
        internal static BoundedContext BoundedContext = BoundedContext.NotSet;
        internal static Environment Environment = Environment.Undetermined;

        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<Application>().To( () => Application);
            builder.Bind<BoundedContext>().To( () => BoundedContext);
            builder.Bind<Environment>().To( () => Environment);
        }
    }
}