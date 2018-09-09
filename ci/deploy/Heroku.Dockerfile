FROM microsoft/dotnet:2.0-aspnetcore-runtime

ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT ${ASPNETCORE_ENVIRONMENT}

COPY app /app
COPY ./migrations.sql /tmp/migrations.sql
WORKDIR /app/

CMD ASPNETCORE_URLS=http://*:$PORT dotnet Borzoo.Web.dll
