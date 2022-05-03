# DotNet Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
SHELL ["/bin/bash", "-c"]
ARG VERSION
ARG TARGETARCH

COPY default.props /app/
COPY versions.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c "Release" -p:Version=${VERSION} -p:RuntimeIdentifier="linux-${TARGETARCH/amd64/x64}" -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
SHELL ["/bin/bash", "-c"]
ARG TARGETARCH

ENV Logging__Console__FormatterName=""
ENV DOLITTLE__RUNTIME__EVENTSTORE__BACKWARDSCOMPATIBILITY__VERSION="V7"

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
    && echo "deb [ arch=$TARGETARCH ] https://repo.mongodb.org/apt/ubuntu bionic/mongodb-org/4.2 multiverse" > /etc/apt/sources.list.d/mongodb-org-4.2.list \
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
    && mongo admin --eval 'db.shutdownServer()'

VOLUME /data/db /data/configdb

# Add Tini to get a real init process
ADD "https://github.com/krallin/tini/releases/download/v0.19.0/tini-$TARGETARCH" /usr/bin/tini
RUN chmod +x /usr/bin/tini

# Create entrypoint that runs both MongoDB and Runtime
COPY Docker/Development/docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh

# Expose the ports
EXPOSE 9700 50052 50053 51052 27017

ENTRYPOINT ["/usr/bin/tini", "--", "/bin/bash", "/usr/bin/docker-entrypoint.sh"]
