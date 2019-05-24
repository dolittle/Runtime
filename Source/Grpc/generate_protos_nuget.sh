#!/bin/bash

# Based on https://github.com/grpc/grpc/blob/master/src/csharp/generate_proto_csharp.sh

find ./Nuget -type f -not -path "*Protobuf/Conversion*" -exec rm {} +
mkdir -p Nuget/Protobuf

PROTOC=~/.nuget/packages/grpc.tools/1.8.0/tools/macosx_x64/protoc

$PROTOC -I./ -I../../Protobuf/ --csharp_out ./Nuget/Protobuf  ../../Protobuf/system/*.proto
cd Interaction
./generate_protos_nuget.sh
cd ..