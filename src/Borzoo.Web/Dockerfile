FROM microsoft/aspnetcore:2.0

WORKDIR /app
COPY bin/publish/Release/ .
COPY migrations.sql ./data/
COPY borzoo.db data/borzoo.db

ENTRYPOINT [ "dotnet", "Borzoo.Web.dll" ]
