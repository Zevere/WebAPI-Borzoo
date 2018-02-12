#!/usr/bin/env bash
set -ex

root_dir=`pwd`

export SQLite_Migrations_Script="$root_dir/src/Borzoo.Data.SQLite/scripts/migrations.sql"
cd "test/Borzoo.Data.SQLite.Tests" &&
    dotnet test --configuration Release --list-tests &&
    dotnet xunit -configuration Release -nobuild -stoponfail -verbose --fx-version 2.0.0
unset SQLite_Migrations_Script

cd "$root_dir"

mongo_container=`docker run -d -p 27017:27017 mongo`
cd "test/Borzoo.Data.Mongo.Tests" &&
    dotnet test --configuration Release --list-tests &&
    dotnet xunit -configuration Release -nobuild -stoponfail -verbose --fx-version 2.0.0
docker rm -f "$mongo_container"

set +x
