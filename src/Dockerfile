#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ./src/config /OpenIdConnectServerMock/config
COPY ./src/keys /OpenIdConnectServerMock/keys


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/OpenIdConnectMockServer.csproj", "src/"]
RUN dotnet restore "src/OpenIdConnectMockServer.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "OpenIdConnectMockServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenIdConnectMockServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=60s --timeout=2s --retries=8 \
      CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "OpenIdConnectMockServer.dll"]
