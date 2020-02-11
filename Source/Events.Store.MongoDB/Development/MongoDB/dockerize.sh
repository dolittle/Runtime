#!/bin/bash

docker build -t dolittle/mongodb:4.2.2-bionic -t dolittle/mongodb:4.2.2 -t dolittle/mongodb:latest .
docker push dolittle/mongodb