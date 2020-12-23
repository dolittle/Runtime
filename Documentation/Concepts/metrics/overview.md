---
title: Overview
description: Overview on metrics
keywords: Metrics
author: Dolittle
weight: 1
aliases:
    - /runtime/runtime/metrics/overview
---

The Dolittle runtime exposes metrics that gives you insight into the runtime.
All metrics are exposed using the [Prometheus data model](https://prometheus.io/docs/concepts/data_model/)
and is available by default through port `9700` on the `/metrics` path.

When running the runtime using Docker, you simply expose the default port `9700`:

```shell
$ docker run -p 9700:9700 dolittle/runtime
```

You can navigate your browser to [http://localhost:9700](http://localhost:9700) and see
the different collectors that are [provided]({{< relref providing_metrics >}}).

