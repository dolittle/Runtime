<h1 align="center"><img src="Documentation/dolittle_negativ_horisontal_RGB.svg" alt="Dolittle"></h1>

<h4 align="center">
    <a href="https://dolittle.io">Documentation</a> |
    <a href="https://dolittle.io/docs/tutorials/getting_started/">Tutorial</a> |
    <a href="https://github.com/dolittle/DotNet.SDK">C# SDK</a> |
</h4>

---

<p align="center">
    <a href="https://hub.docker.com/r/dolittle/runtime"><img src="https://img.shields.io/docker/v/dolittle/runtime?label=dolittle%2Fruntime&logo=docker&sort=semver" alt="Latest Docker image"></a>
    <a href="https://github.com/dolittle/Runtime/actions?query=workflow%3ARuntime"><img src="https://github.com/dolittle/Runtime/workflows/Runtime/badge.svg" alt="Build status"></a>
    <a href="https://github.com/dolittle/Runtime/actions?query=workflow%3ARuntime"><img src="https://github.com/dolittle/Runtime/workflows/Documentation/badge.svg" alt="Documentation build status"></a>
</p>


Dolittle is a decentralized, distributed, event-driven microservice platform built to harness the power of events.

The Runtime is the backend of our system and manages connections from the SDKs and Runtimes to the Event Store. It's called the Runtime as it's what runs and powers the SDK's to do their job.

# Get Started
- Try our [tutorial](https://dolittle.io/docs/tutorials/)
- Check out our [documentation](https://dolittle.io)


# Images

There are 2 different types of images. The `Production` images contain the Runtime. The `Development` images have the Runtime and a MongoDB instance running on port `27017` for easier development. We also have arm64 variants of both images.

| Type | Version                                                                                                                                                                                          |
| ------- |--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Production | [![Docker](https://img.shields.io/docker/v/dolittle/runtime/latest?label=dolittle%2Fruntime%3Alatest&logo=docker&color=blue)](https://hub.docker.com/r/dolittle/runtime)                                    |
| Development | [![Docker](https://img.shields.io/docker/v/dolittle/runtime/latest-development?label=dolittle%2Fruntime%3Alatest-development&logo=docker&color=blue)](https://hub.docker.com/r/dolittle/runtime) |

# Development

**Build**:

From project root:

```shell
dotnet build
```

**Run**:

From project root:

```shell
cd Source/Server
dotnet run
```

Configuration files are in `Source/Server/.dolittle/`

**Tests**:

 From project root:

```shell
dotnet test
```

## Building the Docker image

You build all images from the project root:

**Production**:

```shell
docker build -t dolittle/runtime -f ./Docker/Production/Dockerfile .
```

**ARM64 Production**:

```shell
docker build -t dolittle/runtime:arm64 -f ./Docker/ARM64Production/Dockerfile .
```

**Development**:

```shell
docker build -t dolittle/runtime:development -f ./Docker/Development/Dockerfile .
```

**ARM64 Development**:

```shell
docker build -t dolittle/runtime:arm64-development -f ./Docker/ARM64Development/Dockerfile .
```

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
