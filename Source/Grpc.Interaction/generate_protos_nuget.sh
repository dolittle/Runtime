#!/bin/bash

# Based on https://github.com/grpc/grpc/blob/master/src/csharp/generate_proto_csharp.sh

find ./Nuget -type f -not -path "*Protobuf/Conversion*" -exec rm {} +
mkdir -p Nuget/Grpc && mkdir -p Nuget/Protobuf

PROTOC=~/.nuget/packages/grpc.tools/2.23.0/tools/macosx_x64/protoc
PLUGIN=~/.nuget/packages/grpc.tools/2.23.0/tools/macosx_x64/grpc_csharp_plugin

$PROTOC -I./ -I../../Protobuf/ --csharp_out ./Nuget/Protobuf  ../../Protobuf/dolittle/interaction/events.relativity/*.proto ../../Protobuf/system/*.proto --grpc_out ./Nuget/Grpc --plugin=protoc-gen-grpc=$PLUGIN
