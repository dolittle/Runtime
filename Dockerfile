FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

ARG CONFIGURATION

VOLUME [ "/app/data" ]

COPY default.props ./
COPY versions.props ./
COPY Runtime.sln ./
COPY Source ./Source/
COPY Specifications ./Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c $CONFIGURATION -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as base

ARG CONFIGURATION=Release

RUN echo Configuration = $CONFIGURATION

RUN if [ "$CONFIGURATION" = "Debug" ] ; then apt-get update && \
    apt-get install -y --no-install-recommends unzip procps && \
    rm -rf /var/lib/apt/lists/* \
    ; fi

RUN if [ "$CONFIGURATION" = "debug" ] ; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg ; fi

WORKDIR /app
COPY --from=build-env /app/Source/Server/out ./
COPY --from=build-env /app/Source/Server/.dolittle ./.dolittle

EXPOSE 9700

ENTRYPOINT ["dotnet", "Dolittle.Runtime.Server.dll"]