namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.DependencyInversion;
    using Dolittle.Runtime.Events.Processing;
    using System;

    /// <summary>
    /// Bindings for hooking up Events.Processing Bindings
    /// </summary>
    public class Bindings : ICanProvideBindings 
    {
        /// <inheritdoc />
        public void Provide(IBindingProviderBuilder builder)
        {
            // new BindingBuilder(Binding.For(typeof(Func<IFetchUnprocessedEvents>))).To(() => Container.Get<IFetchUnprocessedEvents> ).Build();
            // new BindingBuilder(Binding.For(typeof(Func<IEventProcessorOffsetRepository>))).To(() => Container.Get<IEventProcessorOffsetRepository> ).Build();
        }
    }
}