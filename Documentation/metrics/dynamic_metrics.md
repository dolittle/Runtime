---
title: Dynamic Metrics
description: How to provide dynamic metrics that gets exposed
keywords: Metrics
author: Dolittle
weight: 3
---
Sometimes you don't know ahead of time what metrics you will be exposing.
Instead of using the [provider model]({{< relref providing_metrics >}}),
you can simply take a dependency to the `IMetricFactory` and create
collectors on the fly.

{{% notice note %}}
The underlying metric system will reuse the collector based on the name
and matching labels and will therefor not impact memory consumption.
{{% /notice %}}

Below is an example of how a tenant specific counter could be created and
used:

```csharp
using Dolittle.Runtime.Metrics;

public class MySystemThatDoesSomething
{
    readonly IMetricFactory _metricFactory;
    readonly IExecutionContextManager _executionContextManager;

    public MySystemThatDoesSomething(
        IMetricFactory metricFactory,
        IExecutionContextManager executionContextManager)
    {
        _metricFactory = metricFactory;
        _executionContextManager = executionContextManager;
    }

    public void DoSomething()
    {
        var counter = _metricFactory.Counter("MyCounter", "This is my counter", "tenant");
        counter.WithLabels(_executionContextManager.Current.Tenant.ToString()).Inc();
    }
}
```
