# Borzoo

A [Zevere GraphQL Web API](https://github.com/Zevere/Zevere-Specs/) implementation in ASP.NET Core C#.

## Gettings Started

This is a Web API with only 1 GraphQL endpoint so

1. Clone dependency projects: `git submodule update --init --recursive`
1. Open the solution in a .NET IDE such as Visual Studio 2017 or Rider
1. Press _Run_!
1. Hit [http://localhost:5000/zv/GraphiQL](http://localhost:5000/zv/GraphiQL)
1. Execute a GraphQL query like:

```graphql
{
  user(userId: "bobby") {
    firstName lastName joinedAt lists { id }
  }
}
```

> Application uses SQLite database by default so it is a painless start. You can change storage type to Mongo Db by modifying `appsettings`.

## Development

- Repository pattern for data storage
  - SQLite implementation
  - Mongo Db implementation
- GraphQL API
- ASP.NET Core Web API to expose GraphQL
- Test Driven Development

## Deployment

CI/CD pipeline is set with the help of [AppVeyor](https://www.appveyor.com/), [Travis-CI](https://travis-ci.org/), and Docker.

### Continuous Integration

#### AppVeyor

All commits are built on AppVeyor. It indicates there are no compile errors. See [`appveyor file`](./.appveyor.yml).

#### Travis-CI

All commits are built and then tested on Travis-CI. This happens inside a Docker container on Travis-CI servers. See [`travis file`](./.travis.yml).

> Pull Requests are not built on Travis-CI due to potential risks of exposing environment variables configured on Travis-CI.

All the [scripts for build, test, and deploy](./scripts) are written in JavaScript and require NodeJS to run.

> [ShellJS](https://github.com/shelljs/shelljs) scripts are good to run on any operating system that is supported by NodeJS. This means we no longer need to be worry about whether we deploy to a Unix-like or a Windows machine.

##### Tests

There are different kinds of tests such as:

- SQLite Repository Tests
- Mongo Db Repository Tests
- Web API Unit Tests
- Web API Systems Integration Tests

Testing framework in use is mainly [xUnit](https://github.com/xunit/xunit/).

Some tests such as systems integration tests take advantage of Docker-in-Docker approach where a temporary Mongo container runs in background to store data.

### Continuous Delivery

Deployment only happens if commit is pushed on `master` branch. If all the tests pass(process exits with return code of 0), deployment process begins. Application environment is set to _Staging_.

#### Staging Server

Deployment server needs to be accessible from Travis-CI container and have a Docker daemon. See [Ubuntu Server Setup](./scripts/deploy/ubuntu-server-setup.md) for more info.

#### Depoloyment Settings

As mentioned in [`travis file`](./.travis.yml), some environment variables should be set to connect to deployment server from Travis-CI build container. Use [`gen-docker-certs.js`](./scripts/deploy/travis-ci/gen-docker-certs.js) script to create a JSON-serialized values for them.

```bash
# in /path/to/docker-certs directory
node /path/to/WebAPI-Borzoo/scripts/deploy/travis-ci/gen-docker-certs.js
```

> This script assumes all Docker daemon certificate files are in one directory. Refer to [Ubuntu server setup file](./scripts/deploy/ubuntu-server-setup.md).

#### Docker Compose

Docker Compose deploys multiple containers at the same time. (See [`docker-compose.yml`](./scripts/deploy/docker-compose.yml))

- Web app ([borzoo.web](./scripts/deploy/Borzoo.Web.Dockerfile))
- Nginx web server

#### Nginx Configurations

Nginx acts as a reverse proxy here. Nginx container loads the configurations file on the host at `/var/nginx/nginx.conf`. See the [sample `nginx.conf`](./scripts/deploy/nginx.conf).