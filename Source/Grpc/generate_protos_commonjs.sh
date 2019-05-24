#!/bin/bash

# Based on https://github.com/grpc/grpc/blob/master/src/csharp/generate_proto_csharp.sh

rm -rf CommonJs
mkdir -p CommonJs/Protobuf

PROTOC=~/.nuget/packages/grpc.tools/1.8.0/tools/macosx_x64/protoc

$PROTOC -I./ -I../../Protobuf/ --js_out=import_style=commonjs,binary:./CommonJs/Protobuf ../../Protobuf/system/*.proto
cd Interaction
./generate_protos_commonjs.sh
cd ..