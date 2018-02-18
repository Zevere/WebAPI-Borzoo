FROM microsoft/aspnetcore:2.0

COPY ./Release/ /app/
COPY ./migrations.sql /tmp/migrations.sql

ENTRYPOINT [ "dotnet", "/app/Borzoo.Web.dll" ]
