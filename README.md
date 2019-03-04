# Runtime

## Cloning

This repository has sub modules, clone it with:

```shell
$ git clone --recursive <repository url>
```

If you've already cloned it, you can get the submodules by doing the following:

```shell
$ git submodule update --init --recursive
```

## Building

All the build things are from a submodule.
To build, run one of the following:

Windows:

```shell
$ Build\build.cmd
```

Linux / macOS

```shell
$ Build\build.sh
```

## Packages

| Platform | Production   | From CI  |
| ------- | ------- | ------ |
| .NET Runtime | [![NuGet](https://img.shields.io/nuget/v/dolittle.Runtime.Events.svg)](https://www.nuget.org/packages?q=dolittle.Runtime) | [![MyGet](https://img.shields.io/myget/dolittle/vpre/dolittle.Runtime.Events.svg)](https://www.myget.org/gallery/dolittle) |

## Build Status

| Project | Windows | Linux / macOS |
| -------- | ------ | ------------- |
| .NET Runtime | [![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/83b4tlt2euskb582?svg=true)](https://ci.appveyor.com/project/Dolittle/dotnet-runtime) [![AppVeyor tests](https://img.shields.io/appveyor/tests/Dolittle/dotnet-runtime.svg)]() | [![Travis Build Status](https://travis-ci.org/dolittle/DotNET.Runtime.svg?branch=master)](https://travis-ci.org/dolittle/DotNET.Runtime) |

## Visual Studio

You can open the `.sln` file in the root of the repository and just build directly.

## VSCode

From the `Build` submdoule there is also a .vscode folder that gets a symbolic link for the root. This means you can open the
root of the repository directly in Visual Studio Code and start building. There are quite a few build tasks, so click F1 and type "Run Tasks" and select the "Tasks: Run Tasks"
option and then select the build task you want to run. It is folder sensitive and will look for the nearest `.csproj` file based on the file you have open.
If it doesn't find it, it will pick the `.sln` file instead.

## More details

To learn more about the projects of Dolittle and how to contribute, please go [here](https://github.com/dolittle/Home).

## Getting Started

Go to our [documentation site](http://www.dolittle.io) and learn more about the project and how to get started.
Samples can also be found [here](https://github.com/Dolittle-Samples).
You can find entropy projects [here](https://github.com/Dolittle-Entropy).

Test 
