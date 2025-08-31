# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY ./src/OpenChat.PlaygroundApp /source/OpenChat.PlaygroundApp

WORKDIR /source/OpenChat.PlaygroundApp

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

WORKDIR /app

COPY --from=build /app .

RUN chown $APP_UID /app

USER $APP_UID

ENTRYPOINT ["dotnet", "OpenChat.PlaygroundApp.dll"]