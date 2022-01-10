# DotNet Build
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-arm64v8 AS dotnet-build
ARG CONFIGURATION=Release
ARG VERSION

COPY default.props /app/
COPY versions.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
# Aiming to get dotnet to create the arm binary
# Reference: https://gist.github.com/alexellis/264ac0fc28b5ae97397039854550c4a9
RUN dotnet publish -c ${CONFIGURATION} -p:Version=${VERSION} -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm64v8
ARG CONFIGURATION=Release
ARG VERSION

RUN echo Configuration = ${CONFIGURATION}
RUN echo Version = ${VERSION}

# https://danielstoyanoff.medium.com/apple-silicon-docker-dotnet-grpc-is-that-compatible-8a05f1a71d89
RUN apt-get update \
 && DEBIAN_FRONTEND=noninteractive \
    apt-get install --no-install-recommends --assume-yes \
      protobuf-compiler-grpc
ENV PROTOBUF_PROTOC=/usr/bin/protoc

RUN if [ "$CONFIGURATION" = "Debug" ] ; then apt-get update && \
    apt-get install -y --no-install-recommends unzip procps curl protobuf-compiler-grpc && \
    rm -rf /var/lib/apt/lists/* \
    ; fi

RUN if [ "$CONFIGURATION" = "Debug" ] ; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg ; fi

WORKDIR /app
COPY --from=dotnet-build /app/Source/Server/out ./
COPY --from=dotnet-build /app/Source/Server/.dolittle ./.dolittle
COPY Docker/Production/.dolittle ./.dolittle

EXPOSE 9700 50052 50053 51052

ENTRYPOINT ["dotnet", "Dolittle.Runtime.Server.dll"]
