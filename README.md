<h1 align="center"><img src="Documentation/dolittle_negativ_horisontal_RGB.svg" alt="Dolittle"></h1>

<h4 align="center">
    <a href="https://dolittle.io">Documentation</a> |
    <a href="https://dolittle.io/docs/tutorials/getting_started/">Tutorial</a> |
    <a href="https://github.com/dolittle/DotNet.SDK">C# SDK</a> |
    <a href="https://github.com/dolittle/JavaScript.SDK">JavaScript SDK</a>
</h4>

---

<p align="center">
    <a href="https://hub.docker.com/r/dolittle/runtime"><img src="https://img.shields.io/docker/v/dolittle/runtime?label=dolittle%2Fruntime&logo=docker&sort=semver" alt="Latest Docker image"></a>
    <a href="https://github.com/dolittle/Runtime/actions?query=workflow%3ARuntime"><img src="https://github.com/dolittle/Runtime/workflows/.NET%20Docker%20Image%20CI/CD/badge.svg" alt="Build status"></a>
</p>


Dolittle is a decentralized, distributed, event-driven microservice platform built to harness the power of events.

The Runtime is the backend of our system and manages connections from the SDKs and Runtimes to the Event Store. It's called the Runtime as it's what runs and powers the SDK's to do their job.

# Get Started
- Try our [tutorial](https://dolittle.io/docs/tutorials/)
- Check out our [documentation](https://dolittle.io)

## Packages

| Platform | Version |
| ------- | ------- |
| Docker | [![Docker](https://img.shields.io/docker/v/dolittle/runtime?label=dolittle%2Fruntime&logo=docker&sort=semver)](https://hub.docker.com/r/dolittle/runtime) |

## Building
```shell
dotnet build
```

## Running
```shell
cd Source/Server
dotnet run
```

Configuration files are in `Source/Server/.dolittle/`

## Visual Studio

You can open the `.sln` file in the root of the repository and just build directly.

## VSCode

We have readymade tasks for VSCode. Press `F1` and type `Run Tasks` and select `Tasks: Run Tasks` to see the tasks.
They are folder sensitive and will look for the nearest `.csproj` file based on the file you have open.
If it doesn't find it, it will pick the `.sln` file instead.


## More

Go to our [documentation site](http://www.dolittle.io) and learn more about the project and how to get started.
Samples can be found in [dolittle-samples](https://github.com/Dolittle-Samples).
Our entropy projects are in [dolittle-entropy](https://github.com/Dolittle-Entropy).

# Issues and Contributing
Issues and contributions are always welcome!

To learn how to contribute, please read our [contributing](https://dolittle.io/docs/contributing/) guide.
