FROM microsoft/aspnetcore:2.0

ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT ${ASPNETCORE_ENVIRONMENT}

WORKDIR /app/

COPY ./Release/ /app/
COPY ./migrations.sql /tmp/migrations.sql

ENTRYPOINT [ "dotnet", "/app/Borzoo.Web.dll" ]
