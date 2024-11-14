# DotNet Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet-build
SHELL ["/bin/bash", "-c"]
ARG VERSION
ARG TARGETARCH

COPY default.props /app/
COPY versions.props /app/
COPY Directory.Packages.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c "Release" -p:Version=${VERSION} -f net9.0 -p:RuntimeIdentifier="linux-${TARGETARCH/amd64/x64}" -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
SHELL ["/bin/bash", "-c"]
ARG TARGETARCH

ENV Logging__Console__FormatterName=""



ENV \
    MONGODB_VERSION=7.0 \
    MONGODB_PORT=27017

# Install MongoDB and tini
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        gnupg \
        tini \
    # Add MongoDB repository
    && curl -fsSL https://pgp.mongodb.com/server-${MONGODB_VERSION}.asc | gpg -o /usr/share/keyrings/mongodb-server-${MONGODB_VERSION}.gpg --dearmor \
    && echo "deb [signed-by=/usr/share/keyrings/mongodb-server-${MONGODB_VERSION}.gpg] http://repo.mongodb.org/apt/debian bookworm/mongodb-org/${MONGODB_VERSION} main" | tee /etc/apt/sources.list.d/mongodb-org-${MONGODB_VERSION}.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        mongodb-org \
    && rm -rf /var/lib/apt/lists/*


# Create directories for MongoDB
RUN mkdir -p /data/db /data/configdb \
    && chown -R mongodb:mongodb /data/db /data/configdb

# Copy MongoDB configuration
COPY Docker/Development/mongod.conf /etc/mongod.conf
RUN chown mongodb:mongodb /etc/mongod.conf



# Create entrypoint that runs both MongoDB and Runtime
COPY Docker/Development/docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh

WORKDIR /app
COPY --from=dotnet-build /app/Source/Server/out ./
COPY --from=dotnet-build /app/Source/Server/.dolittle ./.dolittle

# Expose the ports
EXPOSE 9700 50052 50053 51052 27017

ENTRYPOINT ["tini", "-v", "--", "docker-entrypoint.sh"]
