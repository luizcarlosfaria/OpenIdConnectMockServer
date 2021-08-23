# OpenId Connect Server Mock

![Run Tests badge](https://github.com/Soluto/oidc-server-mock/workflows/Run%20Tests/badge.svg)

This project allows you to run configurable mock server with OpenId Connect functionality.

This is the sample of using the server in `docker-compose` configuration:

```yaml
  version: '3'
  services:
    oidc-server-mock:
      container_name: oidc-server-mock
      image: soluto/oidc-server-mock
      ports:
        - "4011:80"
      environment:
        ASPNETCORE_ENVIRONMENT: Development
        SERVER_OPTIONS_INLINE: |
          {
            "AccessTokenJwtType": "JWT",
            "Discovery": {
              "ShowKeySet": true
            },
            "Authentication": {
              "CookieSameSiteMode": "Lax",
              "CheckSessionCookieSameSiteMode": "Lax"
            }
          }
        ACCOUNT_OPTIONS_INLINE: |
          {
            "AutomaticRedirectAfterSignOut": true
          }
        API_SCOPES_INLINE: |
          [
            {
              "Name": "some-app-scope-1"
            },
            {
              "Name": "some-app-scope-2"
            }
          ]
        API_RESOURCES_INLINE: |
          [
            {
              "Name": "some-app",
              "Scopes": ["some-app-scope-1", "some-app-scope-2"]
            }
          ]
        USERS_CONFIGURATION_INLINE: |
          [
            {
              "SubjectId":"1",
              "Username":"User1",
              "Password":"pwd",
              "Claims": [
                {
                  "Type": "name",
                  "Value": "Sam Tailor"
                },
                {
                  "Type": "email",
                  "Value": "sam.tailor@gmail.com"
                },
                {
                  "Type": "some-api-resource-claim",
                  "Value": "Sam's Api Resource Custom Claim"
                },
                {
                  "Type": "some-api-scope-claim",
                  "Value": "Sam's Api Scope Custom Claim"
                },
                {
                  "Type": "some-identity-resource-claim",
                  "Value": "Sam's Identity Resource Custom Claim"
                }
              ]
            }
          ]
        CLIENTS_CONFIGURATION_PATH: /tmp/config/clients-config.json
      volumes:
        - .:/tmp/config:ro
```

When `clients-config.json` is as following:

```json
[{
        "ClientId": "implicit-mock-client",
        "Description": "Client for implicit flow",
        "AllowedGrantTypes": [
            "implicit"
        ],
        "AllowAccessTokensViaBrowser": true,
        "RedirectUris": [
            "http://localhost:3000/auth/oidc",
            "http://localhost:4004/auth/oidc"
        ],
        "AllowedScopes": [
            "openid",
            "profile",
            "email"
        ],
        "IdentityTokenLifetime": 3600,
        "AccessTokenLifetime": 3600
    },
    {
        "ClientId": "client-credentials-mock-client",
        "ClientSecrets": [
          "client-credentials-mock-client-secret"
        ],
        "Description": "Client for client credentials flow",
        "AllowedGrantTypes": [
            "client_credentials"
        ],
        "AllowedScopes": [
            "some-app"
        ],
        "ClientClaimsPrefix": "",
        "Claims": [
            {
                "Type": "string_claim",
                "Value": "string_claim_value"
            },
            {
                "Type": "json_claim",
                "Value": "['value1', 'value2']",
                "ValueType": "json"
            }
        ]

    }
]
```

Clients configuration should be provided. Test user configuration is optional (used for implicit flow only).

There are two ways to provide configuration for supported scopes, clients and users. You can either provide it inline as environment variable:

* `SERVER_OPTIONS_INLINE`
* `ACCOUNT_OPTIONS_INLINE`
* `API_SCOPES_INLINE`
* `USERS_CONFIGURATION_INLINE`
* `CLIENTS_CONFIGURATION_INLINE`
* `API_RESOURCES_INLINE`
* `IDENTITY_RESOURCES_INLINE`

   or mount volume and provide the path to configuration json as environment variable:

* `SERVER_OPTIONS_PATH`
* `ACCOUNT_OPTIONS_PATH`
* `API_SCOPES_PATH`
* `USERS_CONFIGURATION_PATH`
* `CLIENTS_CONFIGURATION_PATH`
* `API_RESOURCES_PATH`
* `IDENTITY_RESOURCES_PATH`


## Base path

The server can be configured to run with base path. So all the server endpoints will be also available with some prefix segment.
For example `http://localhost:8080/my-base-path/.well-known/openid-configuration` and `http://localhost:8080/my-base-path/connect/token`.
Just set `BasePath` property in `ASPNET_SERVICES_OPTIONS_INLINE/PATH` env var.

## Custom endpoints

### User management

Users can be added (in future also removed and altered) via `user management` endpoint.

* Create new user: `POST` request to `/api/v1/user` path.
  The request body should be the `User` object. Just as in `USERS_CONFIGURATION`.
  The response is subjectId as sent in request.

* Get user: `GET` request to  `/api/v1/user/{subjectId}` path.
  The response is `User` object

* Update user `PUT` request to `/api/v1/user` path. (**Not implemented yet**)
  The request body should be the `User` object. Just as in `USERS_CONFIGURATION`.
  The response is subjectId as sent in request.
  > If user doesn't exits it will be created.

* Delete user: `DELETE` request to  `/api/v1/user/{subjectId}` path.  (**Not implemented yet**)
  The response is `User` object

## HTTPS

To use `https` protocol with the server just add the following environment variables to the `docker run`/`docker-compose up` command, expose ports and mount volume containing the pfx file:

```yaml
environment:
  ASPNETCORE_URLS: https://+:443;http://+:80
  ASPNETCORE_Kestrel__Certificates__Default__Password: <password for pfx file>
  ASPNETCORE_Kestrel__Certificates__Default__Path: /path/to/pfx/file
volumes:
  - ./local/path/to/pfx/file:/path/to/pfx/file:ro
ports:
  - 8080:80
  - 8443:443
```

---

## Cookie SameSite mode

Since Aug 2020 Chrome has a new [secure-by-default model](https://blog.chromium.org/2019/10/developers-get-ready-for-new.html) for cookies, enabled by a new cookie classification system. Other browsers will join in near future.

There are two ways to use `oidc-server-mock` with this change.

1. Run the container with HTTPS enabled (see above).
2. Change cookies `SameSite` mode from default `None` to `Lax`. To do so just add the following to `SERVER_OPTIONS_INLINE` (or the file at `SERVER_OPTIONS_PATH`):

```javascript
{
  // Existing configuration
  // ...
  "Authentication": {
    "CookieSameSiteMode": "Lax",
    "CheckSessionCookieSameSiteMode": "Lax"
  }
}
```

## Contributing

### Requirements

1. [Docker](https://www.docker.com/) (version 18.09 or higher)

2. [NodeJS](https://nodejs.org/en/) (version 10.0.0 or higher)

### Getting started

1. Clone the repo:

      ```sh
      git clone git@github.com:Soluto/oidc-server-mock.git
      ```

2. Install `npm` packages (run from `/e2e` folder):

    ```sh
    npm install
    ```

    > Note: During the build of Docker image UI source code is fetched from [github](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/tree/main). If you experience some issues on project compile step of Docker build or on runtime try to change the branch or commit in the [script](./src/getmain.sh).

3. Run tests:

    ```sh
    npm run test
    ```

## Used by

1. [Tweek](https://github.com/Soluto/tweek) blackbox [tests](https://github.com/Soluto/tweek-blackbox).

2. [Stitch](https://github.com/Soluto/Stitch) e2e tests.

# About this fork

This fork configure default parameters. Is awesome to get started with minimal footprint.

## Default configuration

```dockerfile
ENV ACCOUNT_OPTIONS_PATH        =./config/_account-options.json
ENV SERVER_OPTIONS_PATH         =./config/_server-options.json
ENV USERS_CONFIGURATION_PATH    =./config/users.json
ENV API_RESOURCES_PATH          =./config/resources.json
ENV CLIENTS_CONFIGURATION_PATH  =./config/clients.json
```

On this image, config path is ```/OpenIdConnectServerMock/config/```

## Volume for Keys
RSA keys is generated inside ```/OpenIdConnectServerMock/keys/```

## UI Customization

### Autologin
On login page, all users are listed and you can login clicking in the respective card without use Username and Password. 

This approach reduce timing finding credentials , it's a huge help on live stream sessions and demonstrations.

### Disable validation on /diagnostics

By default access to /diagnostics is only accepted if you are on 127.0.0.1, anybody else is blocked to access this route, including us when using docker.