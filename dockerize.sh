#!/bin/bash
export VERSION=$(git tag --sort=-version:refname | head -1)
docker build --no-cache -f ./Dockerfile -t dolittle/runtime . --build-arg CONFIGURATION="Release"
docker tag dolittle/runtime dolittle/runtime:$VERSION
docker push dolittle/runtime:latest
docker push dolittle/runtime:$VERSION