#!/usr/bin/env bash
set -e
script_path="$( cd "$(dirname "$0")" ; pwd -P )"
root_dir="$script_path/../.."
source "$script_path/../.logging.sh"

cd "$root_dir"

say "Restore NuGet packages"
dotnet restore --disable-parallel --no-cache

say "Build solution"
dotnet build --configuration Release --no-restore
