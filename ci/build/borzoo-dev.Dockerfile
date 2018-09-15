FROM microsoft/dotnet:2.1-aspnetcore-runtime

COPY app /app
COPY migrations.sql /tmp/
WORKDIR /app/

ENV ASPNETCORE_ENVIRONMENT=Development
ENV BORZOO_SETTINGS='{"data":{"sqlite":{"migrations":"/tmp/migrations.sql"}}}'

CMD ASPNETCORE_URLS=http://+:${PORT:-80} dotnet Borzoo.Web.dll