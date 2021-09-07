
# OpenIdConnect Mock Server

![Run Tests badge](https://github.com/luizcarlosfaria/OpenIdConnectMockServer/workflows/Run%20Tests/badge.svg)

Esse projeto tem a função de te ajudar a construir demonstrações, provas de conceito, lives em qualquer tipo de cenário em que ter diversos usuários seja útil para demonstrar perfis diferentes.

A ideia é permitir que você tenha todos os dados para login em arquivos de configuração ou variáveis de ambiente, e ao invés de precisar digitar usuário e senha você simplesmente seleciona com qual usuário você quer logar para demonstrar aalgo.

Esse é um exemplo de `docker-compose.override.yaml`, nesse caso usando os dados default.

```yaml
version: '3.4'

services:
  identity_mock:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:12080;https://+:12443
    ports:
      - "12080:12080"
      - "12443:12443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro
    networks:
     app:
        aliases:
          - localhost10.gago.io

  example:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:13080;https://+:13443
      - bearer__Authority=https://localhost10.gago.io:12443
      - oidc__Authority=https://localhost10.gago.io:12443
    ports:
      - "13080:13080"
      - "13443:13443"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
      - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro
    networks:
     app:
        aliases:
          - localhost1.gago.io


networks:
  app:
    driver: bridge  
      
```

As configurações estão dispostas em pares, [configuration_name]_INLINE e [configuration_name]_PATH. 

 [configuration_name]_INLINE tem maior prioridade e é usado para você descrever inline todo o JSON de configuração.
 
 [configuration_name]_PATH é a segunda estratégia que usa arquivos de configuração. Você deve especificar um path.


Major configurations are organized in [configuration_name]_INLINE and  [configuration_name]_PATH, using all default configuration strategy of ASP .NET. EnvironmentVariables, appsettings.json etc.

The configuration mechanism will try use _INLINE version of configuration and if it empty, will use _PATH version. If _PATH version is empty, some configs will be filled with default value.

There are two ways to provide configuration for supported scopes, clients and users. You can either provide it inline as environment variable:

* `ASPNET_SERVICES_OPTIONS_INLINE`
* `ASPNET_SERVICES_OPTIONS_PATH`
* `SERVER_OPTIONS_INLINE` 
* `SERVER_OPTIONS_PATH` **Default**: `./config/_server-options.json`
* `ACCOUNT_OPTIONS__INLINE`
* `ACCOUNT_OPTIONS__PATH` **Default**: `./config/_account-options.json`
* `SERVER_CORS_ALLOWED_ORIGINS_INLINE` 
* `SERVER_CORS_ALLOWED_ORIGINS_PATH`
* `API_SCOPES_INLINE`
* `API_SCOPES_PATH` **Default**: `./config/api.scopes.json`
* `API_RESOURCES_INLINE`
* `API_RESOURCES_PATH` **Default**: `./config/api.resources.json`
* `CLIENTS_CONFIGURATION_INLINE`
* `CLIENTS_CONFIGURATION_PATH` **Default**: `./config/clients.json`
* `USERS_CONFIGURATION_INLINE`
* `USERS_CONFIGURATION_PATH` **Default**: `./config/users.json`
* `IDENTITY_RESOURCES_INLINE`
* `IDENTITY_RESOURCES_PATH`


### Getting started

1. Clone o repositório:

      ```sh
      git clone https://github.com/luizcarlosfaria/OpenIdConnectMockServer.git
      ```

2. Instale os pacotes `npm` (run from `/e2e` folder):

    ```sh
    npm install
    ```

3. Execute os testes:

    ```sh
    npm run test
    ```

# About this fork

This fork configure default parameters. Is awesome to get started with minimal footprint.

On this image, config path is ```/OpenIdConnectServerMock/config/```

## Volume for Keys
RSA keys is generated inside ```/OpenIdConnectServerMock/keys/```

## UI Customization

### Autologin
On login page, all users are listed and you can login clicking in the respective card without use Username and Password. 

This approach reduce timing finding credentials , it's a huge help on live stream sessions and demonstrations.

### Disable validation on /diagnostics

By default access to /diagnostics is only accepted if you are on 127.0.0.1, anybody else is blocked to access this route, including us when using docker.
