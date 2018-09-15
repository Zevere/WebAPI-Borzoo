FROM microsoft/dotnet:2.1.4-aspnetcore-runtime

COPY app /app
COPY migrations.sql /var/data/sqlite/
WORKDIR /app/

ENV ASPNETCORE_ENVIRONMENT=Development
ENV BORZOO_SETTINGS='{"data":{"sqlite":{"migrations":"/var/data/sqlite/migrations.sql"}}}'

CMD ASPNETCORE_URLS=http://+:${PORT:-80} dotnet Borzoo.Web.dll