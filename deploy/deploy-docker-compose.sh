#!/usr/bin/env bash
set -e

script_path="$( cd "$(dirname "$0")" ; pwd -P )"
repo_path="$script_path/.."

color_green=$(tput setaf 2)
color_normal=$(tput sgr0)
function say {
    if [ $# -ne 1 ]; then
        echo "Incorect number of arguments"
        exit 1
    fi
    printf "\n~~~> ${color_green}$1\n\n${color_normal}"
}


say "Publish app"
cd "$repo_path/src/Borzoo.Web/"
publish_path="bin/publish/Release"
rm -r "$publish_path" || true
dotnet publish --configuration Release --output "$publish_path"


say "Docker compose up"
cd "$repo_path/deploy"
docker-compose rm -f

cp -v \
    "$repo_path/src/Borzoo.Data.SQLite/scripts/migrations.sql" \
    "$repo_path/deploy/Borzoo.Web.Dockerfile" \
    "$repo_path/src/Borzoo.Web/bin/publish/"
docker-compose build --force-rm --no-cache
rm -v "$repo_path/src/Borzoo.Web/bin/publish/Borzoo.Web.Dockerfile" "$repo_path/src/Borzoo.Web/bin/publish/migrations.sql"

docker-compose up -d
docker-compose logs --follow
