# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:3.1 as source
ARG target="Release"

WORKDIR /src

COPY ./OpenIdConnectServerMock.csproj ./OpenIdConnectServerMock.csproj
RUN dotnet restore

COPY . .

RUN dotnet publish -c $target -o obj/docker/publish

RUN cp -r /src/obj/docker/publish /OpenIdConnectServerMock

# Stage 2: Release
FROM mcr.microsoft.com/dotnet/aspnet:3.1 as release
ARG target="Release"

RUN if [ $target = "Debug" ]; then apt-get update && apt-get install unzip && rm -rf /var/lib/apt/lists/* && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg; fi

COPY --from=source /OpenIdConnectServerMock /OpenIdConnectServerMock
COPY --from=source /src/config /OpenIdConnectServerMock/config
COPY --from=source /src/keys /OpenIdConnectServerMock/keys
WORKDIR /OpenIdConnectServerMock

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ACCOUNT_OPTIONS_PATH=./config/_account-options.json
ENV SERVER_OPTIONS_PATH=./config/_server-options.json
ENV USERS_CONFIGURATION_PATH=./config/users.json
ENV API_RESOURCES_PATH=./config/resources.json
ENV CLIENTS_CONFIGURATION_PATH=./config/clients.json

EXPOSE 80

HEALTHCHECK --interval=60s --timeout=2s --retries=8 \
      CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "OpenIdConnectServerMock.dll" ]