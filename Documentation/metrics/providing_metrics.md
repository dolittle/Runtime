---
title: Providing Metrics
description: How to provide metrics that gets exposed
keywords: Metrics
author: Dolittle
weight: 2
aliases:
    - /runtime/runtime/metrics/providing_metrics
---
The type of metrics that are exposed is extensible. Any part of the runtime
and extensions to the runtime can provide metrics that they independently
own and update at their own will.

To provide metrics, all you need to do is implement the `ICanProvideMetrics`
interface found in the [Metrics](https://www.nuget.org/packages/Dolittle.Runtime.Metrics/)
package.

```csharp
using Dolittle.Runtime.Metrics;
using Prometheus;

public class MyMetrics : ICanProvideMetrics
{
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        return new Collector[] {

        };
    }
}
```

The `IMetricFactory` wraps the [Prometheus.NET API](https://github.com/prometheus-net/prometheus-net)
and gives you functionality for creating:

* [Counter](https://prometheus.io/docs/concepts/metric_types/#counter)
* [Gauge](https://prometheus.io/docs/concepts/metric_types/#gauge)
* [Histogram](https://prometheus.io/docs/concepts/metric_types/#histogram)
* [Summary](https://prometheus.io/docs/concepts/metric_types/#summary)

{{% notice note %}}
Recommend reading the [best practices](https://prometheus.io/docs/practices/instrumentation/#counter-vs.-gauge-vs.-summary)
for when to use which type of `Collector`
{{% /notice %}}

## Exposing Metrics to the outside

The provider itself is only responsible for creating the different `Collectors`, to use the
`Collectors` we need them to be exposed in a good way to the outside world. This can be achieved
by exposing public properties on the provider itself and make sure we have it as a singleton
for us to be able use them across the system.

{{% notice warning %}}
The `Collectors` are not per tenant. This means they are to be considered global variables.
Look at [dynamic metrics]({{< relref dynamic_metrics >}}) for an example of how you could
have tenant specific `Collectors`.
{{% /notice %}}

The following example shows how you can achieve this:

```csharp
using Dolittle.Execution;
using Dolittle.Runtime.Metrics;
using Prometheus;

[Singleton]
public class MyMetrics : ICanProvideMetrics
{
    public Counter MyCounter { get; private set; }

    public Gauge MyGauge { get; private set; }

    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        MyCounter = metricFactory.Counter("MyCounter", "This is my first counter");
        MyGauge = metricFactory.Gauge("MyGauge", "This is my first gauge");

        return new Collector[] {
            MyCounter,
            MyGauge
        };
    }
}
```

You can find out more on how to work with the different collector types in
the [Prometheus.NET documentation](https://github.com/prometheus-net/prometheus-net).

## Labels

Metrics can also be labeled, which gives you the ability to have well known
metrics with different properties making them unique. Read more about how this
is done [here](https://github.com/prometheus-net/prometheus-net#labels).

See also the best practices on [naming](http://prometheus.io/docs/practices/naming/)
and [labels](http://prometheus.io/docs/practices/instrumentation/#use-labels).
Names and labels combines into a unique key and makes for unique time series.
This has a performance and data amount implication for the scraper and indexer.
