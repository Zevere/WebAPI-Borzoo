# Borzoo

A [Zevere GraphQL Web API](https://github.com/Zevere/Zevere-Specs/) implementation in ASP.NET Core C#.

## Deployment

CI/CD pipeline is set with the help of AppVeyor, Travis-CI, and Docker.

### CI

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

### CD

Deployment only happens if commit is pushed on `master` branch. If all the tests pass(process exits with return code of 0), deployment begins.

### Server
