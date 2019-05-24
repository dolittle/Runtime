#!/bin/bash

# Based on https://github.com/grpc/grpc/blob/master/src/csharp/generate_proto_csharp.sh

rm -rf CommonJs
mkdir -p CommonJs/Grpc && mkdir -p CommonJs/Protobuf

PROTOC=~/.nuget/packages/grpc.tools/1.8.0/tools/macosx_x64/protoc
PLUGIN=$PWD/../node_modules/.bin/grpc_tools_node_protoc_plugin

$PROTOC -I./ -I../../../Protobuf/ --js_out=import_style=commonjs,binary:./CommonJs/Protobuf  ../../../Protobuf/dolittle/interaction/events.relativity/*.proto --grpc_out ./Commonjs/Grpc --plugin=protoc-gen-grpc=$PLUGIN
