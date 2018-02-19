#!/usr/bin/env bash
set -e
script_path="$( cd "$(dirname "$0")" ; pwd -P )"
root_dir="$script_path/../.."
source "$script_path/../.logging.sh"


say "Run SQLite tests"
export SQLite_Migrations_Script="$root_dir/src/Borzoo.Data.SQLite/scripts/migrations.sql"
cd "$root_dir/test/Borzoo.Data.Tests.SQLite"
dotnet test --list-tests
dotnet test -c Release
# dotnet xunit -configuration Release -stoponfail -verbose
unset SQLite_Migrations_Script


say "Run Mongo tests"
mongo_container=`docker run -d -p 27017:27017 mongo`
cd "$root_dir/test/Borzoo.Data.Tests.Mongo"
dotnet xunit -configuration Release -stoponfail -verbose
docker rm -f "$mongo_container"


say "Run unit tests"
cd "$root_dir/test/Borzoo.Web.Tests.Unit"
dotnet xunit -configuration Release -stoponfail -verbose


say "Run integration tests"
cd "$root_dir/test/Borzoo.Web.Tests.Integ"
dotnet test --list-tests
dotnet test -c Release
# dotnet xunit -configuration Release -nobuild -stoponfail -verbose
