#!/bin/bash

# Based on https://github.com/grpc/grpc/blob/master/src/csharp/generate_proto_csharp.sh

PROTOC=~/.nuget/packages/grpc.tools/1.8.0/tools/macosx_x64/protoc
PLUGIN=~/.nuget/packages/grpc.tools/1.8.0/tools/macosx_x64/grpc_csharp_plugin

$PROTOC -I./ --csharp_out ./  tunnel.proto --grpc_out ./ --plugin=protoc-gen-grpc=$PLUGIN
