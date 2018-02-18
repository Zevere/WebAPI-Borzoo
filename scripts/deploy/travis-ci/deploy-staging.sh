#!/usr/bin/env bash
set -e
script_path="$( cd "$(dirname "$0")" ; pwd -P )"
root_dir="$script_path/../../.."
source "$script_path/../../.logging.sh"

if [ "$TRAVIS_PULL_REQUEST" != "false" ]
then
  say "Skip deployment for pull request"
  exit
fi
if [ "$TRAVIS_BRANCH" != "dev" ]
then
  say "Skip deployment from branch $TRAVIS_BRANCH"
  exit
fi

say "Read Docker configurations"
export DOCKER_HOST="$BORZOO_DOCKER_HOST"
export DOCKER_TLS_VERIFY=1
export DOCKER_CERT_PATH="$root_dir/docker-certs"

[ ! -d "$DOCKER_CERT_PATH" ] && mkdir "$DOCKER_CERT_PATH"
node "$script_path/read-docker-certs.js"

say "Check Docker connection"
docker version

bash "$script_path/../deploy-docker-compose.sh"
