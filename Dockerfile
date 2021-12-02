# https://itnext.io/smaller-docker-images-for-asp-net-core-apps-bee4a8fd1277

FROM alpine:latest as build
RUN apk add --no-cache libstdc++ libintl
WORKDIR /app
EXPOSE 80
COPY WebAPI/*.csproj .
RUN dotnet restore WebAPI/*.csproj
COPY . .
RUN dotnet publish WebAPI/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet","Appneuron.AuthServer.Api.dll" ]