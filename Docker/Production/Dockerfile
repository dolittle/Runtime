# DotNet Build
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS dotnet-build
ARG CONFIGURATION=Release
ARG VERSION

COPY default.props /app/
COPY versions.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c ${CONFIGURATION} -p:Version=${VERSION} -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
ARG CONFIGURATION=Release
ARG VERSION

RUN echo Configuration = ${CONFIGURATION}
RUN echo Version = ${VERSION}

RUN apt-get update && apt-get install -y libc-dev

RUN if [ "$CONFIGURATION" = "Debug" ] ; then apt-get update && \
    apt-get install -y --no-install-recommends unzip procps curl && \
    rm -rf /var/lib/apt/lists/* \
    ; fi

RUN if [ "$CONFIGURATION" = "Debug" ] ; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg ; fi

WORKDIR /app
COPY --from=dotnet-build /app/Source/Server/out ./
COPY --from=dotnet-build /app/Source/Server/.dolittle ./.dolittle

EXPOSE 9700 50052 50053 51052

ENTRYPOINT ["dotnet", "Dolittle.Runtime.Server.dll"]
