// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using Prometheus;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Metrics.for_MetricProviders;

public class when_providing_from_two_providers_with_two_collectors_each
{
    static MetricProviders providers;
    static Mock<IMetricFactory> metric_factory;
    static IEnumerable<Collector> collectors;

    static Collector first_provider_first_collector;
    static Collector first_provider_second_collector;
    static Collector second_provider_first_collector;
    static Collector second_provider_second_collector;

    Establish context = () =>
    {
        var firstProvider = new Mock<ICanProvideMetrics>();
        var secondProvider = new Mock<ICanProvideMetrics>();

        metric_factory = new Mock<IMetricFactory>();

        providers = new MetricProviders(new []{ firstProvider.Object, secondProvider.Object}, metric_factory.Object);

        first_provider_first_collector = Prometheus.Metrics.CreateCounter("FirstFirst", "");
        first_provider_second_collector = Prometheus.Metrics.CreateCounter("FirstSecond", "");
        second_provider_first_collector = Prometheus.Metrics.CreateCounter("SecondFirst", "");
        second_provider_second_collector = Prometheus.Metrics.CreateCounter("SecondSecond", "");

        firstProvider.Setup(_ => _.Provide(metric_factory.Object)).Returns(new[]
        {
            first_provider_first_collector,
            first_provider_second_collector
        });

        secondProvider.Setup(_ => _.Provide(metric_factory.Object)).Returns(new[]
        {
            second_provider_first_collector,
            second_provider_second_collector
        });
    };

    Because of = () => collectors = providers.Provide();

    It should_return_all_collectors = () => collectors.ShouldContainOnly(new[]
    {
        first_provider_first_collector,
        first_provider_second_collector,
        second_provider_first_collector,
        second_provider_second_collector
    });
}