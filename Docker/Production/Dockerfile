# DotNet Build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
ARG VERSION
ARG TARGETARCH

COPY default.props /app/
COPY versions.props /app/
COPY Runtime.sln /app/
COPY Source /app/Source/
COPY Specifications /app/Specifications/

WORKDIR /app/Source/Server
RUN dotnet restore
RUN dotnet publish -c "Release" -p:Version=${VERSION} -p:RuntimeIdentifier="linux-$TARGETARCH" -o out

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

ENV LOGGING__CONSOLE__FormatterName=""

WORKDIR /app
COPY --from=dotnet-build /app/Source/Server/out ./
COPY --from=dotnet-build /app/Source/Server/.dolittle ./.dolittle
COPY Docker/Production/.dolittle ./.dolittle

EXPOSE 9700 50052 50053 51052

ENTRYPOINT ["dotnet", "Dolittle.Runtime.Server.dll"]
