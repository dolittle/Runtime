# DotNet Build
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-arm64v8 AS dotnet-build
ARG CONFIGURATION=Release

COPY default.props /app/
COPY versions.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c ${CONFIGURATION} -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm64v8
ARG CONFIGURATION=Release

# https://danielstoyanoff.medium.com/apple-silicon-docker-dotnet-grpc-is-that-compatible-8a05f1a71d89
RUN apt-get update \
 && DEBIAN_FRONTEND=noninteractive \
    apt-get install --no-install-recommends --assume-yes \
      protobuf-compiler-grpc libc-dev
ENV PROTOBUF_PROTOC=/usr/bin/protoc

WORKDIR /app
COPY --from=dotnet-build /app/Source/Server/out ./
COPY --from=dotnet-build /app/Source/Server/.dolittle ./.dolittle

# Install MongoDB dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        wget \
        gnupg \
    && rm -rf /var/lib/apt/lists/*

# Hack to get MongoDB to install even without systemctl
# See https://www.julienrouse.com/blog/systemctl_not_found_while_installing_mongodb_server/#spoiler-a-solution
RUN ln -s /bin/true /usr/local/sbin/systemctl
RUN ln -s /bin/true /usr/local/bin/systemctl
RUN ln -s /bin/true /usr/sbin/systemctl
RUN ln -s /bin/true /usr/bin/systemctl
RUN ln -s /bin/true /sbin/systemctl
RUN ln -s /bin/true /bin/systemctl

# Install MongoDB
RUN wget -qO - https://www.mongodb.org/static/pgp/server-4.2.asc | apt-key add - \
    && echo "deb [ arch=arm64 ] https://repo.mongodb.org/apt/ubuntu bionic/mongodb-org/4.2 multiverse" > /etc/apt/sources.list.d/mongodb-org-4.2.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        mongodb-org-server \
        mongodb-org-shell \
    && rm -rf /var/lib/apt/lists/*

# Setup MongoDB as single-node replicaset
RUN mkdir -p /data/db /data/configdb \
    && chown -R mongodb:mongodb /data/db /data/configdb \
    && mongod --logpath /var/log/mongodb/initdb.log --replSet "rs0" --bind_ip 0.0.0.0 --fork \
    && mongo --eval 'rs.initiate({_id: "rs0", members: [{ _id: 0, host: "localhost:27017"}]})' \
    && mongod --shutdown

VOLUME /data/db /data/configdb

# Add Tini to get a real init process
ADD https://github.com/krallin/tini/releases/download/v0.19.0/tini-arm64 /usr/bin/tini
RUN chmod +x /usr/bin/tini

# Create entrypoint that runs both MongoDB and Runtime
COPY Docker/ARM64Development/docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh

# Expose all the ports that are usefull
EXPOSE 1880 27017 9007 50052 50053 9700 81

ENTRYPOINT ["/usr/bin/tini", "--", "/bin/bash", "/usr/bin/docker-entrypoint.sh"]
