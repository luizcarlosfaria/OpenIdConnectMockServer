
# OpenIdConnect Mock Server

[![Build and push new image version](https://github.com/luizcarlosfaria/OpenIdConnectMockServer/actions/workflows/tag.yaml/badge.svg)](https://github.com/luizcarlosfaria/OpenIdConnectMockServer/actions/workflows/tag.yaml)

Esse projeto tem a função de te ajudar a construir demonstrações, provas de conceito, lives em qualquer tipo de cenário em que ter diversos usuários seja útil para demonstrar perfis diferentes.

A ideia é permitir que você tenha todos os dados para login em arquivos de configuração ou variáveis de ambiente, e ao invés de precisar digitar usuário e senha você simplesmente seleciona com qual usuário você quer logar para demonstrar algo.

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

As configurações estão dispostas em pares, [configuration_name]_INLINE e [configuration_name]_PATH. Primeiro vou tentar obter a versão INLINE da configuração, caso não esteja configurado, usarei a versão PATH, obtendo o arquivo e carregando para a memória. 

O conteúdo INLINE é um JSON enquanto o conteúdo PATH é um path de um arquivo JSON.

Essas configurações seguem o padrão do ASP .NET, com EnvironmentVariables, appsettings.json etc...

## Configurações

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

[Configurações de Exemplo](https://github.com/luizcarlosfaria/OpenIdConnectMockServer/tree/master/src/config)


# O motivo do FORK

## 1 - Login Facilitado
Com base em uma lista de usuários previamente cadastrados, eu queria poder entregar uma UI em que ao invés de digitar um usuário e senha o usuário simplesmente escolhesse entre algum da lista.
No passado já perdi muito tempo tentando lembrar usuário e senha e fora de um contexto produtivo (produção), credenciais são irrelevantes nesse contexto.

## 2 - O foco não é segurança
Uma coisa importante é que o foco não é a segurança em si. 
O foco aqui são os projetos que precisam de perfis diferentes e para isso precisam de um serviço de autenticação.

## 3 - Evitando gambiarras 
Isolar um IDP dá a possibilidade de desenhar soluções compatíveis com Keycloak e o próprio Identity Server.

# Onde será usado?
## 1 - Cursos
### 1.1 - RabbitMQ
No curso de RabbitMQ para aplicações .NET temos a demanda de trabalhar Event Driven Architecture, e nosso case é uma Exchange, eu preciso lidar com diversos perfis para isso.
### 1.2 - Docker Definitivo / O Roadmap
Já o que diz respeito ao Docker Definitivo / O Roadmap temos demandas das mais variadas sobre microsserviço e poder contar com esse desenho ajuda.
Outro assunto é SAAS Maturity Model, um tema que gostaria de apresentar para eles mostrando como implementar o level 4, desmistificando a ideia de incompatibilidade entre os níveis 3 e 4.
## 2 Material para a comunidade
Tem muitos casos interessantes a demonstrar. Até então era trabalhoso demais criar demonstrações que dependessem de um Identity Provider, agora fica mais simples, mais fácil.

